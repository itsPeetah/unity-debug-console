# unity-debug-console

Simple debug console for Unity

# How to install

- Open the Package Manager window (`Window > Package Manager`)
- Click the `+` dropdown in the top-right corner
- Select `Add package from git URL...`
- Paste the git URL of this repository

# Usage

- Add the debug console command attribute to any **public static (void)** method.
  - The method should only have a single `string[]` parameter.
  - The DebugConsoleCommandAttribute class is contained in the ItsPeetah.DebugConsole namespace, you might want to add a `using ItsPeetah.DebugConsole;` instruction at the top of your script
- Add the console prefab to your scene.
- The first word that will be typed into the input field will be the command, any following words will be the arguments.
  - the command should not contain space characters

```cs
...
{
    // Yes!
    [DebugConsoleCommand("cmd1")]
    public static void CommandExample(string[] args){
        ...
    }

    // Yes! (With two aliases)
    [DebugConsoleCommand("cmd2"), DebugConsoleCommand("cmd2-alias")]
    public static void CommandExample2(string[] args){
        ...
    }

    // No!
    [DebugConsoleCommand("cmd3")]
    public void CommandExample3(string[] args){
        ...
    }
}
...
```
