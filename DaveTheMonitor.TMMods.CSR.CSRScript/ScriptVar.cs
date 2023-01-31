using System;

namespace DaveTheMonitor.TMMods.CSR.CSRScript
{
    /// <summary>
    /// Defines a variable for use with a <see cref="Script"/>.
    /// </summary>
    public struct ScriptVar : IEquatable<ScriptVar>, IComparable<ScriptVar>
    {
        /// <summary>
        /// A <see cref="ScriptVar"/> containing a null value.
        /// </summary>
        public static ScriptVar Null => new ScriptVar();
        /// <summary>
        /// A <see cref="ScriptVar"/> containing true.
        /// </summary>
        public static ScriptVar True => new ScriptVar(true);
        /// <summary>
        /// A <see cref="ScriptVar"/> containing false.
        /// </summary>
        public static ScriptVar False => new ScriptVar(false);
        /// <summary>
        /// The type of this variable.
        /// </summary>
        public ScriptVarType Type { get; private set; }
        /// <summary>
        /// True if <see cref="Type"/> == <see cref="ScriptVarType.Null"/>.
        /// </summary>
        public bool IsNull => Type == ScriptVarType.Null;
        /// <summary>
        /// True if <see cref="Type"/> == <see cref="ScriptVarType.Float"/>.
        /// </summary>
        public bool IsFloat => Type == ScriptVarType.Float;
        /// <summary>
        /// True if <see cref="Type"/> == <see cref="ScriptVarType.Bool"/>.
        /// </summary>
        public bool IsBool => Type == ScriptVarType.Bool;
        /// <summary>
        /// True if <see cref="Type"/> == <see cref="ScriptVarType.String"/>.
        /// </summary>
        public bool IsString => Type == ScriptVarType.String;
        /// <summary>
        /// True if <see cref="Type"/> == <see cref="ScriptVarType.Context"/>.
        /// </summary>
        public bool IsContext => Type == ScriptVarType.Context;
        private float _float;
        private string _string;
        private IScriptContext _context;

        /// <summary>
        /// Sets this variable to null.
        /// </summary>
        public void SetNull()
        {
            Type = ScriptVarType.Null;
        }

        /// <summary>
        /// Sets this variable to the specified <see cref="float"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to set this variable to.</param>
        public void SetValue(float value)
        {
            _float = value;
            Type = ScriptVarType.Float;
        }

        /// <summary>
        /// Sets this variable to 1 if <paramref name="value"/> is true, or 0 if it is false.
        /// </summary>
        /// <param name="value">The value to set this variable to.</param>
        /// <remarks>
        /// <para><see cref="bool"/> script variables do not exist. Instead, > 0 is true and &lt;= 0 is false. This is also enforced at compile-time.</para>
        /// </remarks>
        public void SetValue(bool value)
        {
            _float = value ? 1 : 0;
            Type = ScriptVarType.Float;
        }

        /// <summary>
        /// Sets this variable to the specified <see cref="string"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to set this variable to.</param>
        public void SetValue(string value)
        {
            _string = value;
            Type = ScriptVarType.String;
        }

        /// <summary>
        /// Sets this variable to the specified <see cref="IScriptContext"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to set this variable to.</param>
        public void SetValue(IScriptContext value)
        {
            _context = value;
            Type = ScriptVarType.Context;
        }

        /// <summary>
        /// Gets the <see cref="float"/> value of this variable.
        /// </summary>
        /// <param name="runtime">The runtime executing the current script.</param>
        /// <remarks>
        /// <para>If the runtime is supplied, this method can throw a runtime error if this variable is not a float.</para>
        /// </remarks>
        public float GetFloatValue(IScriptRuntime runtime)
        {
            if (!IsFloat)
            {
                runtime?.Error("Invalid Variable Type", "Variable is " + Type + ", expected Float");
                return 0;
            }
            return _float;
        }

        /// <summary>
        /// Gets the <see cref="bool"/> value of this variable.
        /// </summary>
        /// <param name="runtime">The runtime executing the current script.</param>
        /// <remarks>
        /// <para>If the runtime is supplied, this method can throw a runtime error if this variable is not a bool.</para>
        /// </remarks>
        public bool GetBoolValue(IScriptRuntime runtime)
        {
            if (!IsBool)
            {
                runtime?.Error("Invalid Variable Type", "Variable is " + Type + ", expected Bool");
                return false;
            }
            return _float > 0;
        }

        /// <summary>
        /// Gets the <see cref="string"/> value of this variable.
        /// </summary>
        /// <param name="runtime">The runtime executing the current script.</param>
        /// <remarks>
        /// <para>If the runtime is supplied, this method can throw a runtime error if this variable is not a <see cref="string"/>.</para>
        /// </remarks>
        public string GetStringValue(IScriptRuntime runtime)
        {
            if (!IsString)
            {
                runtime?.Error("Invalid Variable Type", "Variable is " + Type + ", expected String");
                return null;
            }
            return _string;
        }

        /// <summary>
        /// Gets the <see cref="IScriptContext"/> value of this variable.
        /// </summary>
        /// <param name="runtime">The runtime executing the current script.</param>
        /// <remarks>
        /// <para>If the runtime is supplied, this method can throw a runtime error if this variable is not a <see cref="IScriptContext"/>.</para>
        /// </remarks>
        public IScriptContext GetContextValue(IScriptRuntime runtime)
        {
            if (!IsContext)
            {
                runtime?.Error("Invalid Variable Type", "Variable is " + Type + ", expected Context");
                return null;
            }
            return _context;
        }

        private static bool CheckTypesForOp(IScriptRuntime runtime, ScriptVar left, ScriptVar right)
        {
            bool leftIsValid = left.IsFloat;
            bool rightIsValid = right.IsFloat;
            if (leftIsValid && rightIsValid)
            {
                return true;
            }

            if (!leftIsValid)
            {
                runtime?.Error("Invalid Operation Types", left.Type + " values cannot be operated on.");
                return false;
            }
            else if (!rightIsValid)
            {
                runtime?.Error("Invalid Operation Types", right.Type + " values cannot be operated on.");
                return false;
            }

            return true;
        }

        private static bool CheckTypesForAdd(IScriptRuntime runtime, ScriptVar left, ScriptVar right)
        {
            if (left.IsString || right.IsString)
            {
                return true;
            }

            bool leftIsValid = left.IsFloat;
            bool rightIsValid = right.IsFloat;
            if (leftIsValid && rightIsValid)
            {
                return true;
            }

            if (!leftIsValid)
            {
                runtime?.Error("Invalid Operation Types", left.Type + " values cannot be operated on.");
                return false;
            }
            else if (!rightIsValid)
            {
                runtime?.Error("Invalid Operation Types", right.Type + " values cannot be operated on.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns <paramref name="left"/> + <paramref name="right"/>. Only <see cref="float"/>s and <see cref="string"/>s can be added.
        /// </summary>
        /// <param name="left">The first variable.</param>
        /// <param name="right">The second variable.</param>
        /// <param name="runtime">The runtime executing the current script. Allows this method to throw runtime errors if the variables are unexpected types.</param>
        /// <returns>A new <see cref="ScriptVar"/> with the result of the operation, or <see cref="Null"/> if the supplied variables are unexpected types.</returns>
        public static ScriptVar Add(ScriptVar left, ScriptVar right, IScriptRuntime runtime = null)
        {
            if (!CheckTypesForAdd(runtime, left, right))
            {
                return new ScriptVar();
            }

            if (left.IsString || right.IsString)
            {
                return new ScriptVar(left.ToString() + right.ToString());
            }
            else
            {
                return new ScriptVar(left.GetFloatValue(runtime) + right.GetFloatValue(runtime));
            }
        }

        /// <summary>
        /// Returns <paramref name="left"/> - <paramref name="right"/>. Only <see cref="float"/>s can be subtracted.
        /// </summary>
        /// <param name="left">The first variable.</param>
        /// <param name="right">The variable to subtract from <paramref name="left"/>.</param>
        /// <param name="runtime">The runtime executing the current script. Allows this method to throw runtime errors if the variables are unexpected types.</param>
        /// <returns>A new <see cref="ScriptVar"/> with the result of the operation, or <see cref="Null"/> if the supplied variables are unexpected types.</returns>
        public static ScriptVar Sub(ScriptVar left, ScriptVar right, IScriptRuntime runtime = null)
        {
            if (!CheckTypesForOp(runtime, left, right))
            {
                return new ScriptVar();
            }

            return new ScriptVar(left.GetFloatValue(runtime) - right.GetFloatValue(runtime));
        }

        /// <summary>
        /// Returns <paramref name="left"/> * <paramref name="right"/>. Only <see cref="float"/>s can be multiplied.
        /// </summary>
        /// <param name="left">The first variable.</param>
        /// <param name="right">The second variable.</param>
        /// <param name="runtime">The runtime executing the current script. Allows this method to throw runtime errors if the variables are unexpected types.</param>
        /// <returns>A new <see cref="ScriptVar"/> with the result of the operation, or <see cref="Null"/> if the supplied variables are unexpected types.</returns>
        public static ScriptVar Multi(ScriptVar left, ScriptVar right, IScriptRuntime runtime = null)
        {
            if (!CheckTypesForOp(runtime, left, right))
            {
                return new ScriptVar();
            }

            return new ScriptVar(left.GetFloatValue(runtime) * right.GetFloatValue(runtime));
        }

        /// <summary>
        /// Returns <paramref name="left"/> / <paramref name="right"/>. Only <see cref="float"/>s can be divided.
        /// </summary>
        /// <param name="left">The numerator.</param>
        /// <param name="right">The denominator.</param>
        /// <param name="runtime">The runtime executing the current script. Allows this method to throw runtime errors if the variables are unexpected types.</param>
        /// <returns>A new <see cref="ScriptVar"/> with the result of the operation, or <see cref="Null"/> if the supplied variables are unexpected types.</returns>
        public static ScriptVar Div(ScriptVar left, ScriptVar right, IScriptRuntime runtime = null)
        {
            if (!CheckTypesForOp(runtime, left, right))
            {
                return new ScriptVar();
            }

            return new ScriptVar(left.GetFloatValue(runtime) / right.GetFloatValue(runtime));
        }

        /// <summary>
        /// Returns <paramref name="left"/> % <paramref name="right"/>. Only valid for <see cref="float"/>s.
        /// </summary>
        /// <param name="left">The dividend.</param>
        /// <param name="right">The divisor.</param>
        /// <param name="runtime">The runtime executing the current script. Allows this method to throw runtime errors if the variables are unexpected types.</param>
        /// <returns>A new <see cref="ScriptVar"/> with the result of the operation, or <see cref="Null"/> if the supplied variables are unexpected types.</returns>
        public static ScriptVar Mod(ScriptVar left, ScriptVar right, IScriptRuntime runtime = null)
        {
            if (!CheckTypesForOp(runtime, left, right))
            {
                return new ScriptVar();
            }

            return new ScriptVar(left.GetFloatValue(runtime) % right.GetFloatValue(runtime));
        }

        /// <summary>
        /// Compares this <see cref="ScriptVar"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to compare to.</param>
        /// <returns>-2 if either this or <paramref name="value"/> are not floats, -1 if this &lt; <paramref name="value"/>, 0 if this == <paramref name="value"/>, 1 is this > <paramref name="value"/>.</returns>
        public int CompareTo(ScriptVar value)
        {
            if (IsNull || IsString || IsContext || IsBool || value.IsNull || value.IsString || value.IsContext || value.IsBool)
            {
                return -2;
            }
            else
            {
                float left = GetFloatValue(null);
                float right = value.GetFloatValue(null);
                if (left < right)
                {
                    return -1;
                }
                else if (left > right)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Compares this <see cref="ScriptVar"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to compare to.</param>
        /// <param name="runtime">The runtime executing the current script. Allows this method to throw runtime errors if the variables are unexpected types.</param>
        /// <returns>-2 if either this or <paramref name="value"/> are not floats, -1 if this &lt; <paramref name="value"/>, 0 if this == <paramref name="value"/>, 1 is this > <paramref name="value"/>.</returns>
        public int CompareTo(ScriptVar value, IScriptRuntime runtime)
        {
            if (IsNull || IsString || IsContext || value.IsNull || value.IsString || value.IsContext)
            {
                runtime?.Error("Invalid Operation Types", Type + " values cannot be operated on.");
                return -2;
            }
            else
            {
                float left = GetFloatValue(null);
                float right = value.GetFloatValue(null);
                if (left < right)
                {
                    return -1;
                }
                else if (left > right)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Returns true if this and <paramref name="value"/> are equal.
        /// </summary>
        /// <param name="value">The value to compare to.</param>
        /// <returns>True if this and <paramref name="value"/> are equal.</returns>
        public bool Equals(ScriptVar value)
        {
            if (Type != value.Type)
            {
                return false;
            }
            else if (IsNull)
            {
                return true;
            }
            else if (IsFloat)
            {
                return GetFloatValue(null) == value.GetFloatValue(null);
            }
            else if (IsString)
            {
                return GetStringValue(null) == value.GetStringValue(null);
            }
            else if (IsContext)
            {
                return GetContextValue(null) == value.GetContextValue(null);
            }
            else if (IsBool)
            {
                return GetBoolValue(null) == value.GetBoolValue(null);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if this and <paramref name="value"/> are equal.
        /// </summary>
        /// <param name="value">The value to compare to.</param>
        /// <returns>True if this and <paramref name="value"/> are equal.</returns>
        public override bool Equals(object value)
        {
            if (value is ScriptVar v)
            {
                return Equals(v);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if <paramref name="left"/> and <paramref name="right"/> are equal.
        /// </summary>
        /// <param name="left">The first variable.</param>
        /// <param name="right">The second variable.</param>
        /// <returns>True if <paramref name="left"/> and <paramref name="right"/> are equal.</returns>
        public static bool operator ==(ScriptVar left, ScriptVar right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true if <paramref name="left"/> and <paramref name="right"/> are not equal.
        /// </summary>
        /// <param name="left">The first variable.</param>
        /// <param name="right">The second variable.</param>
        /// <returns>True if <paramref name="left"/> and <paramref name="right"/> are not equal.</returns>
        public static bool operator !=(ScriptVar left, ScriptVar right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc cref="Add"/>
        public static ScriptVar operator +(ScriptVar left, ScriptVar right)
        {
            return Add(left, right);
        }

        /// <inheritdoc cref="Sub"/>
        public static ScriptVar operator -(ScriptVar left, ScriptVar right)
        {
            return Sub(left, right);
        }

        /// <inheritdoc cref="Multi"/>
        public static ScriptVar operator *(ScriptVar left, ScriptVar right)
        {
            return Multi(left, right);
        }

        /// <inheritdoc cref="Div"/>
        public static ScriptVar operator /(ScriptVar left, ScriptVar right)
        {
            return Div(left, right);
        }

        /// <inheritdoc cref="Mod"/>
        public static ScriptVar operator %(ScriptVar left, ScriptVar right)
        {
            return Mod(left, right);
        }

        /// <summary>
        /// Tests if left &gt; right.
        /// </summary>
        /// <param name="left">The first variable.</param>
        /// <param name="right">The second variable.</param>
        /// <returns>The result.</returns>
        public static bool operator >(ScriptVar left, ScriptVar right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Tests if left &lt; right.
        /// </summary>
        /// <param name="left">The first variable.</param>
        /// <param name="right">The second variable.</param>
        /// <returns>The result.</returns>
        public static bool operator <(ScriptVar left, ScriptVar right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Tests if left &gt;= right.
        /// </summary>
        /// <param name="left">The first variable.</param>
        /// <param name="right">The second variable.</param>
        /// <returns>The result.</returns>
        public static bool operator >=(ScriptVar left, ScriptVar right)
        {
            return left.CompareTo(right) >= 0;
        }

        /// <summary>
        /// Tests if left &lt;= right.
        /// </summary>
        /// <param name="left">The first variable.</param>
        /// <param name="right">The second variable.</param>
        /// <returns>The result.</returns>
        public static bool operator <=(ScriptVar left, ScriptVar right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Converts the value of this variable to its equivalent <see cref="string"/> representation.
        /// </summary>
        /// <returns>The <see cref="string"/> representation of the value of this variable.</returns>
        /// <remarks>
        /// <para>For <see cref="IScriptContext"/> variables, <see cref="IScriptContext.GetName()"/> is called instead of <see cref="object.ToString()"/>.</para>
        /// <para>Returns "null" for null values.</para>
        /// </remarks>
        public override string ToString()
        {
            switch (Type)
            {
                case ScriptVarType.Float: return _float.ToString();
                case ScriptVarType.Bool: return _float > 0 ? "true" : "false";
                case ScriptVarType.String: return _string;
                case ScriptVarType.Context: return _context.GetName();
                default: return "null";
            }
        }

        /// <summary>
        /// Returns the hashcode for the value of this <see cref="ScriptVar"/>.
        /// </summary>
        /// <returns>A 32-bit signed integer hashcode.</returns>
        /// <remarks>
        /// <para>Only the hashcode of the current value is returned. ie. if this <see cref="ScriptVar"/> is a <see cref="float"/>, the hashcode of the float is returned.</para>
        /// </remarks>
        public override int GetHashCode()
        {
            switch (Type)
            {
                case ScriptVarType.Bool: return _float > 0 ? true.GetHashCode() : false.GetHashCode();
                case ScriptVarType.Float: return _float.GetHashCode();
                case ScriptVarType.String: return _string.GetHashCode();
                case ScriptVarType.Context: return _context.GetHashCode();
                default: return 0;
            }
        }

        /// <summary>
        /// Creates a new <see cref="ScriptVar"/> with the specified <see cref="float"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value of this <see cref="ScriptVar"/>.</param>
        public ScriptVar(float value)
        {
            Type = ScriptVarType.Float;
            _float = value;
            _string = null;
            _context = null;
        }

        /// <summary>
        /// Creates a new <see cref="ScriptVar"/> with the value 1 if <paramref name="value"/> is true, or 0 if it is false.
        /// </summary>
        /// <param name="value">The value of this <see cref="ScriptVar"/>.</param>
        /// <remarks>
        /// <para><see cref="bool"/> script variables do not exist. Instead, > 0 is true and &lt;= 0 is false. This is also enforced at compile-time.</para>
        /// </remarks>
        public ScriptVar(bool value)
        {
            Type = ScriptVarType.Bool;
            _float = value ? 1 : 0;
            _string = null;
            _context = null;
        }

        /// <summary>
        /// Creates a new <see cref="ScriptVar"/> with the specified <see cref="string"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value of this <see cref="ScriptVar"/>.</param>
        public ScriptVar(string value)
        {
            Type = ScriptVarType.String;
            _float = 0;
            _string = value;
            _context = null;
        }

        /// <summary>
        /// Creates a new <see cref="ScriptVar"/> with the specified <see cref="IScriptContext"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value of this <see cref="ScriptVar"/>.</param>
        public ScriptVar(IScriptContext value)
        {
            Type = ScriptVarType.Context;
            _float = 0;
            _string = null;
            _context = value;
        }
    }
}
