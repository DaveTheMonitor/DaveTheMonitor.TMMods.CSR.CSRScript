using System.Collections.Generic;

namespace DaveTheMonitor.TMMods.CSR.CSRScript
{
    /// <summary>
    /// A <see cref="IScriptContext"/> that gives scripts access to an <see cref="IList{T}"/>.
    /// </summary>
    public interface IScriptArray : IScriptContext
    {
        /// <summary>
        /// If true, the array cannot be modified by scripts.
        /// </summary>
        bool ReadOnly { get; }
        /// <summary>
        /// The numbers of items in the array.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        /// <param name="runtime">The runtime executing the calling script.</param>
        /// <returns>A variable containing the item.</returns>
        ScriptVar GetItem(int index, IScriptRuntime runtime);
        /// <summary>
        /// Sets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to set.</param>
        /// <param name="value">The value to set the item to. Should match the type of the array.</param>
        /// <param name="runtime">The runtime executing the calling script.</param>
        void SetItem(int index, ScriptVar value, IScriptRuntime runtime);
        /// <summary>
        /// Inserts an item at the specified index.
        /// </summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="value">The value to insert. Should match the type of the array.</param>
        /// <param name="runtime">The runtime executing the calling script.</param>
        void Insert(int index, ScriptVar value, IScriptRuntime runtime);
        /// <summary>
        /// Adds a value to the array.
        /// </summary>
        /// <param name="value">The value to add. Should match the type of the array.</param>
        /// <param name="runtime">The runtime executing the calling script.</param>
        void Add(ScriptVar value, IScriptRuntime runtime);
        /// <summary>
        /// Removes a value from the array, if it is present.
        /// </summary>
        /// <param name="value">The value to remove. Should match the type of the array.</param>
        /// <param name="runtime">The runtime executing the calling script.</param>
        void Remove(ScriptVar value, IScriptRuntime runtime);
        /// <summary>
        /// Removes the value at the specified index.
        /// </summary>
        /// <param name="index">The index to remove the value at.</param>
        /// <param name="runtime">The runtime executing the calling script.</param>
        void RemoveAt(int index, IScriptRuntime runtime);
        /// <summary>
        /// Clears the array.
        /// </summary>
        /// <param name="runtime">The runtime executing the valling script.</param>
        void Clear(IScriptRuntime runtime);
        /// <summary>
        /// Returns the index of the specified item in the array, or -1 if the item is not present.
        /// </summary>
        /// <param name="value">The value to get the index of. Should match the type of the array.</param>
        /// <param name="runtime">The runtime executing the calling script.</param>
        /// <returns>The index of the specified item, or -1 is the item is not present.</returns>
        ScriptVar IndexOf(ScriptVar value, IScriptRuntime runtime);
    }
}
