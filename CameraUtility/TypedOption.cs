namespace CameraUtility
{
    public abstract record TypedOption<T>(
        T Value)
    {
        public static implicit operator T(
            TypedOption<T> typedOption)
        {
            return typedOption.Value;
        }

        public override string ToString()
        {
            return Value!.ToString() ?? string.Empty;
        }
    }
}