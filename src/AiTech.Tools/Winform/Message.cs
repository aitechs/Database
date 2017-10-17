using AiTech.Tools.Properties;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Validator;
using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;


namespace AiTech.Tools.Winform
{

    public enum MessageDialogResult
    {
        None = 0,
        Ok = 1,
        Yes = 2,
        No = 3,
        Cancel = 4,
        Retry = 5,
        Close = 6,
        Ignore = 7,
        Abort = 8
    }


    public static class MessageDialog
    {
        public static void Show(IWin32Window owner, string header, string message, MessageBoxIcon icon = MessageBoxIcon.Exclamation)
        {

            MessageBoxEx.Show(owner, message, header, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1);

        }

        public static void Show(string header, string message, MessageBoxIcon icon = MessageBoxIcon.Exclamation)
        {

            MessageBoxEx.Show(message, header, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1);

        }

        public static void ShowError(Exception ex, Form owner, [CallerMemberName] string caller = "")
        {

            var baseException = ex.GetBaseException();

            var error = "\n" + ex.Message + "\n\n" + ex.GetBaseException().Message;

            var formName = "";
            if (owner != null) formName = owner.Name;

            var stackmsg = baseException.StackTrace;
            var stack = stackmsg.Split('\n');

            var size = 3;

            if (stack.Length < 3) size = stack.Length;


            stackmsg = "Error Details:\n";
            for (var i = 0; i < size; i++)
            {
                stackmsg += "--> " + stack[i] + "\n\n";
            }

            //if (stack.Length != 0) stackmsg = "Error Details:\n" + stack[0] + "\n";


            var info = new TaskDialogInfo()
            {
                Title = "Notification",
                TaskDialogIcon = eTaskDialogIcon.Stop,
                DialogButtons = eTaskDialogButton.Ok,
                Header = "Oops! Something went wrong.",
                Text = error + "\n\n" + stackmsg,
                FooterText = String.Format("Error Source : {0}.{1} : {2}", formName, caller, baseException.Source)
            };

            if (owner == null)
            {
                TaskDialog.Show(info);
                return;
            }

            TaskDialog.Show(owner, info);
        }


        public static MessageDialogResult Ask(string header, string message, eTaskDialogButton defaultButton = eTaskDialogButton.No)
        {
            var info = new TaskDialogInfo()
            {
                Title = "Confirmation",
                TaskDialogIcon = eTaskDialogIcon.Help,
                Header = header,
                Text = message,
                DefaultButton = defaultButton,
                DialogButtons = eTaskDialogButton.Yes | eTaskDialogButton.No

            };

            return (MessageDialogResult)TaskDialog.Show(info);
        }



        public static MessageDialogResult AskToRefresh()
        {
            var info = new TaskDialogInfo()
            {
                Title = "Confirmation",
                TaskDialogIcon = eTaskDialogIcon.Help,
                Header = "Refresh Data from Server?",
                Text = "Refreshing will remove all the pending changes you made? \n\n Do you want to continue?",
                DefaultButton = eTaskDialogButton.No,
                DialogButtons = eTaskDialogButton.Yes | eTaskDialogButton.No
            };
            return (MessageDialogResult)TaskDialog.Show(info);
        }



        public static MessageDialogResult AskToDelete(string msg = "")
        {
            var info = new TaskDialogInfo()
            {
                Title = "Confirmation",
                TaskDialogIcon = eTaskDialogIcon.Help,
                DialogButtons = eTaskDialogButton.Ok,
                Header = "Delete Record",
                Text = $"You are about to <font color='red'><b>DELETE</b></font> record? <br/><br/>{msg}<br/><br/> Do you want to continue?",
                DefaultButton = eTaskDialogButton.No
            };
            info.DialogButtons = eTaskDialogButton.Yes | eTaskDialogButton.No;

            info.DialogColor = eTaskDialogBackgroundColor.Red;

            return (MessageDialogResult)TaskDialog.Show(info);
        }


        public static MessageDialogResult AskToSave()
        {
            //var info = new TaskDialogInfo()
            //{
            //    Title = "Notification",
            //    TaskDialogIcon = eTaskDialogIcon.Help,
            //    Header = "Save Changes?",
            //    Text = "Do you want to save the changes you made?",
            //    DefaultButton = eTaskDialogButton.Yes,
            //    DialogButtons = eTaskDialogButton.Yes | eTaskDialogButton.Cancel | eTaskDialogButton.No
            //};
            //return (MessageDialogResult)TaskDialog.Show(info);

            var result = MessageBoxEx.Show("Do you want to save the changes you made?", "Save Changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            switch (result)
            {
                case DialogResult.Yes:
                    return MessageDialogResult.Yes;
                case DialogResult.No:
                    return MessageDialogResult.No;
                case DialogResult.None:
                    return MessageDialogResult.None;
                case DialogResult.OK:
                    return MessageDialogResult.Ok;
                case DialogResult.Cancel:
                    return MessageDialogResult.Cancel;
                case DialogResult.Abort:
                    return MessageDialogResult.Abort;
                case DialogResult.Retry:
                    return MessageDialogResult.Retry;
                default:
                    return MessageDialogResult.Ignore;
            }

        }


        public static MessageDialogResult PromptSaveChanges(string header, string message, IWin32Window owner = null)
        {
            var info = new TaskDialogInfo();

            var commandOk = new Command()
            {
                Image = (System.Drawing.Image)Resources.ResourceManager.GetObject("Checkmark_24px"),
                Text = @"Save Changes"
            };
            commandOk.Executed += (s, e) => { TaskDialog.Close(eTaskDialogResult.Yes); };

            var commandNo = new Command()
            {
                Image = (System.Drawing.Image)Resources.ResourceManager.GetObject("Delete_24px"),
                Text = @"DO NOT save Changes"
            };
            commandNo.Executed += (s, e) => { TaskDialog.Close(eTaskDialogResult.No); };

            var commandCancel = new Command()
            {
                Image = (System.Drawing.Image)Resources.ResourceManager.GetObject("Unavailable_24px"),
                Text = @"Cancel"
            };
            commandCancel.Executed += (s, e) => { TaskDialog.Close(eTaskDialogResult.Cancel); };


            info.Title = @"Application Message";
            info.TaskDialogIcon = eTaskDialogIcon.Help;
            info.Header = header;
            info.DialogColor = eTaskDialogBackgroundColor.Default;
            info.Text = message;



            //info.DialogButtons = eTaskDialogButton.Yes | eTaskDialogButton.Cancel | eTaskDialogButton.No;
            info.Buttons = new[] { commandOk, commandNo, commandCancel };

            var response = owner == null ? TaskDialog.Show(info) : TaskDialog.Show(owner, info);


            switch (response)
            {
                case eTaskDialogResult.Yes: return MessageDialogResult.Yes;
                case eTaskDialogResult.No: return MessageDialogResult.No;
                default: return MessageDialogResult.Cancel;
            }
        }


        /// <summary>
        /// Show Toast Notification
        /// </summary>
        /// <param name="control"></param>
        /// <param name="errorMessage"></param>
        /// <param name="errorProvider"></param>
        /// <param name="highlighter"></param>
        /// <param name="topMargin"></param>
        /// <param name="focusControl"></param>
        public static void ShowValidationError(Control control, string errorMessage, ErrorProvider errorProvider = null, Highlighter highlighter = null, int topMargin = 5, bool focusControl = true)
        {
            ToastNotification.DefaultToastGlowColor = eToastGlowColor.Red;
            ToastNotification.ToastMargin.Top = topMargin;

            ToastNotification.Show(control.FindForm(), errorMessage, eToastPosition.TopCenter);

            if (errorProvider != null)
                errorProvider.SetError(control, errorMessage);

            if (highlighter != null)
            {
                highlighter.ContainerControl = control.FindForm();
                highlighter.SetHighlightColor(control, eHighlightColor.Red);
            }

            if (focusControl) control.Focus();
        }
    }


}
