public class AllyButterfreeJump : Ally
{
    protected override void ExtraTrailEffects(FollowTowards ft)
    {
        ft.isButterfree = true;
    }
}
