namespace ChessVariantsLogic;

public interface IPattern
{

    int GetXDir();

    int GetYDir();
    int GetMinLength();
    int GetMaxLength();
}