HotkeyTool
=========================
* Registers global Hotkeys to your own Hotkeyfunctions written in C#
* This programm is written in C# and uses .Net Framework 4.

Usage
----------
Implement the interface IHotkeyFunction and start coding your own Hotkeyfunction.

Hotkeyfunctions must always exist in the namespace HotkeyTool.HotKeyFunctions.

Use the method Execute() to launch your actions.

```C#
namespace HotkeyTool.HotKeyFunctions
{
    public class TestFunction : IHotkeyFunction
    {
        public void Execute()
        {
            MessageBox.Show("Hotkey was pressed!");
        }
    }
}
```

Iconsets used:
famfamfam.com Silk Iconset by Mark James
Human-O2 Iconset by Oliver Scholtz

License
----------
Copyright 2012 Richard 'r15ch13' Kuhnt

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  <http://www.apache.org/licenses/LICENSE-2.0>

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS-IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.