using ModestTree;

namespace Zenject.Asteroids
{
    public enum ShipStates
    {
        Moving,
        Dead,
        WaitingToStart,
        Count
    }

    public class ShipStateFactory
    {
        private readonly ShipStateDead.Factory _deadFactory;
        private readonly ShipStateMoving.Factory _movingFactory;
        private readonly ShipStateWaitingToStart.Factory _waitingFactory;

        public ShipStateFactory(
            ShipStateDead.Factory deadFactory,
            ShipStateMoving.Factory movingFactory,
            ShipStateWaitingToStart.Factory waitingFactory)
        {
            _waitingFactory = waitingFactory;
            _movingFactory = movingFactory;
            _deadFactory = deadFactory;
        }

        public ShipState CreateState(ShipStates state)
        {
            switch (state)
            {
                case ShipStates.Dead:
                {
                    return _deadFactory.Create();
                }
                case ShipStates.WaitingToStart:
                {
                    return _waitingFactory.Create();
                }
                case ShipStates.Moving:
                {
                    return _movingFactory.Create();
                }
            }

            throw Assert.CreateException();
        }
    }
}