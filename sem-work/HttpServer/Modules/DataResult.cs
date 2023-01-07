namespace HttpServer;

public class DataResult
{
    public bool IsAllCorrect => IsUserExist && IsEmailCorrect && IsPasswordCorrect && IsNameCorrect && IsSurnameCorrect;
    public bool IsUserExist { get; set; }
    public bool IsEmailCorrect { get; set; }
    public bool IsPasswordCorrect { get; set; }
    public bool IsNameCorrect { get; set; }
    public bool IsSurnameCorrect { get; set; }

    public DataResult(bool isUserExist, bool isEmailCorrect, bool isPasswordCorrect, bool isNameCorrect,
        bool isSurnameCorrect)
    {
        IsUserExist = isUserExist;
        IsEmailCorrect = isEmailCorrect;
        IsPasswordCorrect = isPasswordCorrect;
        IsNameCorrect = isNameCorrect;
        IsSurnameCorrect = isSurnameCorrect;
    }
}