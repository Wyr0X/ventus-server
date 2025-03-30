using System.Collections.Generic;

public class MoveWithUserInputSystem : ISystem
{
    private readonly Engine _engine;

    public MoveWithUserInputSystem(Engine engine)
    {
        _engine = engine;
    }

    public void Run(EntityManager entities)
    {
        Position? moving = null;
        var keys = DirectionKeyEnum.Down;
        switch (keys)
        {
            case DirectionKeyEnum.Up:
                moving = Vec2Utils.Up;
                break;
            case DirectionKeyEnum.Right:
                moving = Vec2Utils.Right;
                break;
            case DirectionKeyEnum.Down:
                moving = Vec2Utils.Down;
                break;
            case DirectionKeyEnum.Left:
                moving = Vec2Utils.Left;
                break;
            default:
                moving = null;
                break;
        }

          foreach (var (component, entity) in entities.Get(typeof(FollowComponent)))
        {
            Movable movable = (Movable)entity.Get(typeof(Movable));

            if (moving != null)
            {
                if (!movable.Moving)
                {
                    movable.Moving = true;
                    entity.Add(new Moving());
                    entities.Dispatch(EntityEvent.StartMoving, entity);
                }

                Heading heading = (Heading)entity.Get(typeof(Heading));
                if (heading.Direction.directionEnum != moving)
                {
                    heading.Direction = moving;
                    entities.Dispatch(EntityEvent.ChangeHeading, entity);
                }
            }
            else if (movable.Moving)
            {
                movable.Moving = false;
                entities.RemoveComponent(entity, typeof(Moving));
                entities.Dispatch(EntityEvent.StopMoving, entity);
            }
        }
    }
}
