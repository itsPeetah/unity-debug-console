using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ItsPeetah.DebugConsole
{
    public class DebugConsole : MonoBehaviour
    {

        private static DebugConsole main;
        public static DebugConsole Main => main;



        private Dictionary<string, Action<string[]>> commands;

        [Header("UI Components")]
        [SerializeField] private GameObject window;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private TextMeshProUGUI output;

        [Header("Settings")]
        [SerializeField, Tooltip("Is the command key case sensitive?")]
        private bool commandKeyCaseSensitive = false;
        [SerializeField, Tooltip("Should override input field submit listeners?")]
        private bool overrideInputListeners = true;

        private bool commandsLoaded = false;
        private bool active;
        public bool Active
        {
            get => active; set
            {
                active = value;
                if (!active)
                {
                    window.SetActive(false);
                }
                else
                {
                    if (!commandsLoaded)
                    {
                        LoadCommandsFromAssemblies();
                        commandsLoaded = true;
                    }

                    input.SetTextWithoutNotify(string.Empty);
                    window.SetActive(true);
                    input.Select();
                }
            }
        }



        private void Awake()
        {
            main = this;
        }

        private void Start()
        {
            Active = false;
            if (overrideInputListeners)
            {
                input.onSubmit.RemoveAllListeners();
                input.onSubmit.AddListener(OnInputSubmit);
            }
            output.SetText(string.Empty);
        }

        [DebugConsoleCommand("t1")]
        public static void Test1(string[] s)
        {
            Debug.Log("Invoked t1");
            DebugConsole.Main.Print("Hello 1");
        }
        [DebugConsoleCommand("t2")]
        public static void Test2(string[] s)
        {
            Debug.Log("Invoked t1");
            DebugConsole.Main.Print("Hello 1");
        }
        [DebugConsoleCommand("t3")]
        public static void Test3(string s)
        {
            Debug.Log("Invoked t1");
            DebugConsole.Main.Print("Hello 3");
        }

        private void LoadCommandsFromAssemblies()
        {
            Debug.Log("Loading debug commands...");
            commands = new Dictionary<string, Action<string[]>>();
            var methods = AppDomain.CurrentDomain.GetAssemblies()
                                   .SelectMany(x => x.GetTypes())
                                   .Where(x => x.IsClass)
                                   .SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.Static))
                                   .Where(x => x.GetCustomAttributes(typeof(DebugConsoleCommandAttribute), false).FirstOrDefault() != null);
            foreach (var method in methods)
            {
                DebugConsoleCommandAttribute attribute = (DebugConsoleCommandAttribute)method.GetCustomAttributes(typeof(DebugConsoleCommandAttribute), false).First();
                string commandKey = attribute.Command;

                if (commands.ContainsKey(commandKey))
                {
                    Debug.LogWarning($"Could not add command \"{commandKey}\" for method \"{method.Name}\": duplicte key!");
                    continue;
                }

                ParameterInfo[] parameters = method.GetParameters();

                if (parameters.Length > 1)
                {
                    Debug.LogWarning($"Could not add command \"{commandKey}\" for method \"{method.Name}\": too many arguments!");
                    continue;
                }

                ParameterInfo param = parameters.First();
                if (!param.ParameterType.Equals(typeof(string[])))
                {
                    Debug.LogWarning($"Could not add command \"{commandKey}\" for method \"{method.Name}\": argument must be of type {typeof(string[])}!");
                    continue;
                }

                Action<string[]> commandDelegate = (Action<string[]>)Delegate.CreateDelegate(typeof(Action<string[]>), method);

                commands.Add(commandKey, commandDelegate);
            }
            Debug.Log("Loaded debug commands.");

        }

        private void OnInputSubmit(string inputText)
        {
            ExecuteCommand(inputText);
            input.SetTextWithoutNotify(string.Empty);
            input.Select();
        }

        private void ExecuteCommand(string command)
        {
            string[] fields = command.Split(' ');
            string cmd = commandKeyCaseSensitive ? fields[0].ToLower() : fields[0];

            if (!commands.ContainsKey(cmd))
            {
                Debug.Log($"Did not find command \"{cmd}\"");
                return;
            }

            string[] args = new string[fields.Length - 1];
            for (int i = 1; i < fields.Length; i++)
            {
                args[i - 1] = fields[i];
            }

            try
            {
                commands[cmd].Invoke(args);
            }
            catch (Exception e)
            {
                Debug.Log($"Could not execute command \"{command}\".\n{e}");
            }
        }

        public void Print(string text)
        {
            output.SetText(text);
        }
    }
}