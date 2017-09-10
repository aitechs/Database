namespace AiTech.Tools.Winform
{
    public interface ISave
    {
        bool FileSave();
        DirtyFormHandler DirtyStatus { get; }
    }




}
