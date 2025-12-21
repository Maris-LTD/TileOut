namespace GameModules.Core
{
    public enum ModuleScope
    {
        Project = 0, // Global scope, accessible throughout the project
        Scene = 1, // Scene scope, accessible within the current scene
        GameObject = 2, // GameObject scope, accessible within a specific GameObject
    }
}