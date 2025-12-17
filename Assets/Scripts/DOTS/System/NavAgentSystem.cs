using DOTS.Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.AI;

[UpdateAfter(typeof(UnitMoverSystem))]
[UpdateAfter(typeof(RandomWalkingSystem))]
public partial struct NavAgentSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var inputData = SystemAPI.GetSingleton<InputData>();
        var moveDirection = new float3(inputData.Movement.x, 0, inputData.Movement.y);
        
        foreach (var (navAgent, localTransform, target, unitMover, randomWalking, enabledRandomWalking, entity) 
                 in SystemAPI.Query<RefRW<NavAgentComponent>, RefRW<LocalTransform>, RefRO<Target>, RefRW<UnitMover>, RefRO<RandomWalking>, EnabledRefRW<RandomWalking>>()
                     .WithPresent<RandomWalking>()
                     .WithEntityAccess())
        {
            var wayPointBuffer = SystemAPI.GetBuffer<WaypointBuffer>(entity);

            if (target.ValueRO.targetEntity == Entity.Null && unitMover.ValueRO.reachedTarget &&
                navAgent.ValueRO.currentWaypoint == (wayPointBuffer.Length - 1) && 
                math.lengthsq(randomWalking.ValueRO.targetPosition - unitMover.ValueRO.targetPosition) <= UnitMoverSystem.ReachedTargetPositionDistanceSQ)
            {
                enabledRandomWalking.ValueRW = true;
                continue;
            }

            if (target.ValueRO.targetEntity != Entity.Null && math.lengthsq(moveDirection) > 0.01f ||
                unitMover.ValueRO.reachedTarget && navAgent.ValueRO.currentWaypoint == wayPointBuffer.Length - 1 ||
                wayPointBuffer.Length == 0)
            {
                navAgent.ValueRW.pathCalculated = false;
                
                if (target.ValueRO.targetEntity != Entity.Null)
                {
                    navAgent.ValueRW.targetPosition = SystemAPI.GetComponentRO<LocalTransform>(target.ValueRO.targetEntity).ValueRO.Position;
                }
                else
                {
                    navAgent.ValueRW.targetPosition = randomWalking.ValueRO.targetPosition;
                }
                CalculatePath(navAgent, localTransform, wayPointBuffer, ref state);
            }
            else if (unitMover.ValueRO.reachedTarget)
            {
                if (navAgent.ValueRO.currentWaypoint + 1 < wayPointBuffer.Length)
                {
                    navAgent.ValueRW.currentWaypoint += 1;
                }
            }
            Move(navAgent, wayPointBuffer, unitMover);
        }
    }

    [BurstCompile]
    private void Move(RefRW<NavAgentComponent> navAgent, DynamicBuffer<WaypointBuffer> waypointBuffer, RefRW<UnitMover> unitMover)
    {
        if (waypointBuffer.IsEmpty) return;
        
        unitMover.ValueRW.targetPosition = waypointBuffer[navAgent.ValueRO.currentWaypoint].wayPoint;
        unitMover.ValueRW.lookPosition = waypointBuffer[navAgent.ValueRO.currentWaypoint].wayPoint;
    }

    [BurstCompile]
    private void CalculatePath(RefRW<NavAgentComponent> navAgent, RefRW<LocalTransform> localTransform, DynamicBuffer<WaypointBuffer> waypointBuffer, ref SystemState state)
    {
        var query = new NavMeshQuery(NavMeshWorld.GetDefaultWorld(), Allocator.TempJob, 1000);

        var fromPosition = localTransform.ValueRO.Position;
        var toPosition = navAgent.ValueRO.targetPosition;

        var extents = new float3(1,1,1);
        var fromLocation = query.MapLocation(fromPosition, extents, 0);
        var toLocation = query.MapLocation(toPosition, extents, 0);

        PathQueryStatus status;
        PathQueryStatus returningStatus;
        int maxPathSize = 100;

        if (query.IsValid(fromLocation) && query.IsValid(toLocation))
        {
            status = query.BeginFindPath(fromLocation, toLocation);
            if (status == PathQueryStatus.InProgress || status == PathQueryStatus.Success)
            {
                status = query.UpdateFindPath(100, out var iterationsPerformed);
                if (status == PathQueryStatus.Success)
                {
                    status = query.EndFindPath(out var pathSize);
                    
                    NativeArray<NavMeshLocation> result = new NativeArray<NavMeshLocation>(pathSize + 1, Allocator.Temp);
                    NativeArray<StraightPathFlags> straightPathFlag = new NativeArray<StraightPathFlags>(maxPathSize, Allocator.Temp);
                    NativeArray<float> vertexSide = new NativeArray<float>(maxPathSize, Allocator.Temp);
                    NativeArray<PolygonId> polygonIds = new NativeArray<PolygonId>(pathSize + 1, Allocator.Temp);
                    int straightPathCount = 0;

                    query.GetPathResult(polygonIds);

                    returningStatus = PathUtils.FindStraightPath
                    (
                        query,
                        fromPosition,
                        toPosition,
                        polygonIds,
                        pathSize,
                        ref result,
                        ref straightPathFlag,
                        ref vertexSide,
                        ref straightPathCount,
                        maxPathSize
                    );

                    if(returningStatus == PathQueryStatus.Success)
                    {
                        waypointBuffer.Clear();

                        foreach (NavMeshLocation location in result)
                        {
                            if (location.position != Vector3.zero)
                            {
                                waypointBuffer.Add(new WaypointBuffer { wayPoint = location.position });
                            }
                        }

                        navAgent.ValueRW.currentWaypoint = 1;//cuz first is current position
                        navAgent.ValueRW.pathCalculated = true;
                    }
                    straightPathFlag.Dispose();
                    polygonIds.Dispose();
                    vertexSide.Dispose();
                }
            }
        }
        query.Dispose();
    }
}