using System.Windows.Forms;

namespace AiTech.Tools.Winform
{



    public static class FormHelper
    {

        public static void AskToSaveOnDirtyClosing(this ISave frm)
        {
            ((Form)frm).FormClosing += (s, e) =>
            {
                switch (e.CloseReason)
                {
                    case CloseReason.MdiFormClosing:
                    case CloseReason.ApplicationExitCall:
                    case CloseReason.UserClosing:
                        //Check for Dirty
                        if (!frm.DirtyStatus.IsDirty) break;

                        var response = MessageDialog.PromptSaveChanges("Changes Detected", "You made changes to the current record. <br/><br/>What do you want to do?", (Form)frm);

                        switch (response)
                        {
                            case MessageDialogResult.Yes:
                                if (!frm.FileSave()) e.Cancel = true;
                                break;

                            case MessageDialogResult.Cancel:
                                e.Cancel = true;
                                break;
                        }
                        break;
                }
            };
        }


        public static void ConvertEnterToTab(this Form form)
        {
            form.KeyPreview = true;
            form.KeyPress += (s, e) =>
            {
                if ((e.KeyChar != (int)Keys.Enter) && (e.KeyChar != (int)Keys.Return)) return;

                //Console.WriteLine("Next Control " + form.ActiveControl.Name); ;
                form.SelectNextControl(form.ActiveControl, true, true, true, true);
                e.Handled = true;
            };
        }







        //public interface ITextField
        //{
        //    string Text { get; set; }
        //}



        //public interface IRecordInfo
        //{
        //    DateTime Created { get; set; }
        //    string CreatedBy { get; set; }
        //    DateTime Modified { get; set; }
        //    string ModifiedBy { get; set; }
        //}


        //public static void ShowFileInfo(IRecordInfo info, ITextField lblCreated, ITextField lblModified)
        //{
        //    const string template = @"<b>{By}</b><br/>
        //                              {Date}<br/>{Time}";

        //    var str = "";

        //    if (info.Created.Year <= 1920) return;

        //    str = template.Replace("{By}", info.CreatedBy);
        //    str = str.Replace("{Date}", info.Created.ToString("dd-MMM-yyyy"));
        //    str = str.Replace("{Time}", info.Created.ToString("hh:mm:ss tt"));

        //    lblCreated.Text = str;

        //    str = template.Replace("{By}", info.ModifiedBy);
        //    str = str.Replace("{Date}", info.Modified.ToString("dd-MMM-yyyy"));
        //    str = str.Replace("{Time}", info.Modified.ToString("hh:mm:ss tt"));

        //    lblModified.Text = str;

        //}
    }



}
