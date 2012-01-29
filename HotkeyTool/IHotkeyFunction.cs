/*
 * Copyright 2012 Richard 'r15ch13' Kuhnt
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace HotkeyTool
{
    /// <summary>
    /// This interface provides an Execute() method for all hotkeyfunctions
    /// </summary>
    public interface IHotkeyFunction
    {
        /// <summary>
        /// Hotkey Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Will be executed when the assign Hotkey is pressed
        /// </summary>
        void Execute();
    }
}
