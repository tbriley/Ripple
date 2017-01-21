using UniRx;
using Zenject;
using AlphaECS;
using System;

namespace AlphaECS
{
    public class AlphaECSInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IMessageBroker>().To<MessageBroker>().AsSingle();
            Container.Bind<IEventSystem>().To<EventSystem>().AsSingle();
            Container.Bind<IIdentityGenerator>().To<SequentialIdentityGenerator>().AsSingle();
            Container.Bind<IPoolManager>().To<PoolManager>().AsSingle();
			Container.Bind<IGroup> ().To<Group> ();
			Container.Bind<GroupFactory> ().To<GroupFactory> ().AsSingle ();
//			Container.BindFactory<Type[], Group, GroupFactory> ();
        }
    }
}