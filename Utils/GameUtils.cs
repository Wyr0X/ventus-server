
using Google.Protobuf.Collections;

public static class GameUtils
{
    public enum KeyEnum
    {
        None = 0,
        W = 1 << 0, // 0001
        D = 1 << 1, // 0010
        S = 1 << 2, // 0100
        A = 1 << 3  // 1000
    }
    internal static List<KeyEnum> ParseKeys(RepeatedField<uint> keys)
    {
        List<KeyEnum> pressedKeys = new List<KeyEnum>();

        foreach (var key in keys)
        {
            KeyEnum pressed = (KeyEnum)key;
            if (pressed != KeyEnum.None)
            {
                pressedKeys.Add(pressed);
            }
        }

        return pressedKeys;
    }

}