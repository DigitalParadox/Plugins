using DigitalParadox.Plugins;

namespace TestInterfaces
{
    public interface ITestPluginB : IPlugin
    {
        void Execute();
        void PreExecute();
        void PostExecute();
    }

    public interface ITestPluginA : IPlugin
    {
        void DoStuff();
    }





}
