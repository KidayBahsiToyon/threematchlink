using Core.ServiceLocator;
using Game.Board;
using Game.Link;
using Game.Managers;
using Game.Services;
using UnityEngine;

namespace Game
{
    public class GameBootstrapper : Bootstrapper
    {
        [SerializeField] private BoardController _board;
        [SerializeField] private LinkController _link;
        [SerializeField] private GameManager _game;

        protected override void Bootstrap()
        {
            Container.ConfigureForScene();
            
            Container
                .Register<IBoardService>(_board)
                .Register<ILinkService>(_link)
                .Register<IGameService>(_game);
        }
    }
}
