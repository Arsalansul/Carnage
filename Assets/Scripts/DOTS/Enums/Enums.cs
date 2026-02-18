public enum Faction
{
    Friendly,
    Enemy
}

public enum EnemyType
{
    none,
    Arachnid,
    Juggernaut,
    Reptile,
    Cockroach,
    Insect,
    Mutant,
    Slug
}

public enum BulletsType
{
    none = 0,
    small = 1,
    explosion = 2,
    spread = 3,
    
    //enemy projectiles
    enemySmall = 11
}

public enum WeaponType
{
    none = 0,
    Mp5 = 1,
    RocketGun = 2,
    M4 = 3,
    Benelli= 4,
    M110 = 5,
    M249 = 6,
    
    //enemy range weapons
    Arachnid = 11,
}

public enum PickupType
{
    heal,
    bomb,
    weapon
}

public enum PoolName
{
    smallBullet,
    explosionBullet,
    enemyBullet,
    firstAid,
    bomb,
    mp5,
    benelliM4,
    m4,
    m249,
    smaw
}