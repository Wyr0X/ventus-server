using System;
using System.Collections.Generic;

public class Game : Engine
{
    private int mouseMoveFrame;
    private bool userHasInteracted = false;




    public override void Start()
    {
        List<ISystem> systems = new List<ISystem>();
        systems.Add(new MoveWithUserInputSystem(this));
        systems.Add(new UpdateMovableSystem());
        systems.Add(new FollowEntitySystem());

        Entities.AddSystems(SystemTrigger.Update,
            systems
        );

    


        // Entities.AddListeners<Character>(new CharacterRenderingSystem(this),
        //     EntityManager.ComponentAdded
        // );

  

        // var player = Player.Create(this, Vec2Utils.Zero);
        // Entities.AddComponents(player, new MoveWithUserInput());

        // Entities.Create(
        //     new Position(0, 0),
        //     new Camera(Vec2.DivF(mainView.Resolution, 2)),
        //     new FollowEntity(player)
        // );

     
    }

    public override void Update()
    {
        Entities.RunSystems(SystemTrigger.Update);
    }

    public override void Render()
    {
        Entities.RunSystems(SystemTrigger.Render);
    }

   

    private void HandleMouseDown(int X, int Y)
    {
        Position pos = new Position(X, Y);
    }

    private void HandleMouseUp(object sender, EventArgs e) { }

    private void HandleMouseMove(int X, int Y)
    {
        Position pos = new Position(X, Y);
    }

    private void HandleMouseLeave(object sender, EventArgs e) { }

    private void UserInteracted()
    {
        userHasInteracted = true;
    }

    private void HandleFocus(object sender, EventArgs e)
    {
        if (!userHasInteracted) return;
    }

    private void HandleBlur(object sender, EventArgs e)
    {
        HandleMouseLeave(sender, e);
    }


    public override void Preload()
    {
        throw new NotImplementedException();
    }
}
