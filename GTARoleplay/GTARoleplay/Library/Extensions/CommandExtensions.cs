using GTANetworkAPI;
using GTANetworkMethods;
using System;
using System.Reflection;

namespace GTARoleplay.Library.Extensions
{
    public static class CommandExtensions
    {
        public static void Register(this Command clientEvent, RuntimeCommandInfo commandInfo, Action action)
            => clientEvent.Register(commandInfo, action.Method);

        public static void Register<T>(this Command clientEvent, RuntimeCommandInfo commandInfo, Action<T> action)
            => clientEvent.Register(commandInfo, action.Method);

        public static void Register<T1, T2>(this Command clientEvent, RuntimeCommandInfo commandInfo, Action<T1, T2> action)
            => clientEvent.Register(commandInfo, action.Method);

        public static void Register<T1, T2, T3>(this Command clientEvent, RuntimeCommandInfo commandInfo, Action<T1, T2, T3> action)
            => clientEvent.Register(commandInfo, action.Method);

        public static void Register<T1, T2, T3, T4>(this Command clientEvent, RuntimeCommandInfo commandInfo, Action<T1, T2, T3, T4> action)
            => clientEvent.Register(commandInfo, action.Method);

        public static void Register<T1, T2, T3, T4, T5>(this Command clientEvent, RuntimeCommandInfo commandInfo, Action<T1, T2, T3, T4, T5> action)
            => clientEvent.Register(commandInfo, action.Method);

        public static void Register(this Command clientEvent, RuntimeCommandInfo commandInfo, MethodInfo methodInfo)
        {
            clientEvent.Register(methodInfo, commandInfo);
        }
    }
}
