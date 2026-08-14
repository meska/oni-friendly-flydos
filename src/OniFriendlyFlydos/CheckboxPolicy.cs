namespace OniFriendlyFlydos
{
    internal static class CheckboxPolicy
    {
        private const int UncheckedState = 0;

        public static bool GetValueAfterClick(int currentState)
        {
            // PLib el segnala lo stato corrente: el click su unchecked vol dir true.
            return currentState == UncheckedState;
        }
    }
}
