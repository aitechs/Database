using DevComponents.DotNetBar.Controls;
using DevComponents.DotNetBar.SuperGrid;
using System;
using System.Windows.Forms;

namespace AiTech.Tools.Winform
{

    public class DirtyFormHandler
    {
        protected readonly ISave _form;
        public bool IsDirty { get; protected set; }

        public event EventHandler DirtyCleared;
        public event EventHandler DirtySet;

        public DirtyFormHandler(ISave form)
        {
            _form = form;
            IsDirty = false;
            SubscribeToControlCollection(((Form)form).Controls);
        }

        public virtual void SetDirty()
        {
            if (IsDirty) return;

            IsDirty = true;



            if (((Form)_form).IsMdiChild)
            {
                //Change Form Caption
                var title = ((Form)_form).Text;
                if (title.Substring(title.Length - 1) != "*")
                    title += " *";
                ((Form)_form).Text = title;
            }

            OnDirtySet();

        }

        public void Clear()
        {
            if (!IsDirty) return;

            IsDirty = false;

            var form = (Form)_form;

            if (form.IsMdiChild)
            {

                var title = form.Text;
                if (title.Substring(title.Length - 1) == "*")
                    form.Text = form.Tag.ToString();
            }

            OnDirtyClear();
        }

        protected void SubscribeToControlCollection(Control.ControlCollection coll)
        {
            foreach (Control c in coll)
            {
                if (c is TextBox ||
                    c is ComboBox)
                    c.TextChanged += (s, e) => SetDirty();

                if (c is DateTimePicker)
                    ((DateTimePicker)c).ValueChanged += (s, e) => SetDirty();

                if (c is CheckBox)
                    (c as CheckBox).CheckedChanged += (s, e) => SetDirty();

                if (c is SwitchButton)
                    (c as SwitchButton).ValueChanged += (s, e) => SetDirty();

                if (c is DevComponents.Editors.DateTimeAdv.DateTimeInput)
                    (c as DevComponents.Editors.DateTimeAdv.DateTimeInput).ValueChanged += (s, e) => SetDirty();


                if (c is SuperGridControl)
                    (c as SuperGridControl).RowMarkedDirty += (s, e) => SetDirty();

                // recurively apply to inner collections
                if (c.HasChildren)
                    SubscribeToControlCollection(c.Controls);
            }
        }


        protected virtual void OnDirtyClear()
        {
            DirtyCleared?.Invoke(this, EventArgs.Empty);
        }


        protected virtual void OnDirtySet()
        {
            DirtySet?.Invoke(this, EventArgs.Empty);
        }
    }



}


