using Game.Gameplay.Entities;
using Game.Gameplay.Services;
using Game.Gameplay.System;
using Game.Gameplay.Views;
using GameModules.Core;
using VContainer;

namespace Game.Gameplay.Installers
{
    [AutoInstall(ModuleScope.Scene, priority: 0)]
    public class GameplayInstaller : IGameModuleInstaller
    {
        public string ModuleName => "Gameplay";

        public void Install(IContainerBuilder builder)
        {
            RegisterTileGrid(builder);
            RegisterTileMovementSystem(builder);
            RegisterTileMapSpawner(builder);
            RegisterGameplayManager(builder);
            RegisterGameplayService(builder);
            InitializeGameplayManager(builder);
        }

        private void RegisterTileGrid(IContainerBuilder builder)
        {
            builder.Register<TileGrid>(Lifetime.Singleton);
        }

        private void RegisterTileMovementSystem(IContainerBuilder builder)
        {
            builder.Register<TileMovementSystem>(Lifetime.Singleton);
        }

        private void RegisterTileMapSpawner(IContainerBuilder builder)
        {
            builder.Register<TileMapSpawner>(Lifetime.Scoped);
        }

        private void RegisterGameplayManager(IContainerBuilder builder)
        {
            builder.Register<GameplayManager>(Lifetime.Scoped);
        }

        private void RegisterGameplayService(IContainerBuilder builder)
        {
            builder.Register<IGameplayService, GameplayService>(Lifetime.Scoped);
        }

        private void InitializeGameplayManager(IContainerBuilder builder)
        {
            builder.RegisterBuildCallback(container =>
            {
                var gameplayManager = container.Resolve<GameplayManager>();
                gameplayManager.Initialize();
            });
        }
    }
}