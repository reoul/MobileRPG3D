public interface IConvertBytes
{
    void ToBytes(WriteMemoryStream writeMemoryStream);
    void ToData (ReadMemoryStream readMemoryStream);
}
