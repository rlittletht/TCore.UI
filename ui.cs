using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace TCore.UI
{
	public class InputBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox textBox1;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button button1, button2, buttonBrowse;
		private System.Windows.Forms.Label m_lbl;

		private bool m_fCanceled = false;
		private string m_sFilter = "All Files (*.*)|*.*";
		private string m_sLabel = null;

		/* I N P U T  B O X */
		/*----------------------------------------------------------------------------
			%%Function: InputBox
			%%Qualified: UI.InputBox.InputBox
			%%Contact: rlittle

		----------------------------------------------------------------------------*/
		private InputBox(string sPrompt, string sText, bool fShowBrowse, bool fHideInput, string sLabel)
		{
			m_sLabel = sLabel;
			InitializeComponent(fShowBrowse, fHideInput);
			if (sText != null)
				textBox1.Text = sText;

			this.Text = sPrompt;
		}

		/* D I S P O S E */
		/*----------------------------------------------------------------------------
			%%Function: Dispose
			%%Qualified: UI.InputBox.Dispose
			%%Contact: rlittle

		----------------------------------------------------------------------------*/
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		/* I N I T I A L I Z E  C O M P O N E N T */
		/*----------------------------------------------------------------------------
			%%Function: InitializeComponent
			%%Qualified: UI.InputBox.InitializeComponent
			%%Contact: rlittle

		----------------------------------------------------------------------------*/
		private void InitializeComponent(bool fShowBrowse, bool fHideInput)
		{
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.m_lbl = new Label();
			this.button1 = new Button();
			this.button2 = new Button();
			this.buttonBrowse = new Button();
			this.SuspendLayout();

			float dxfTextBox = 262;
			if (fShowBrowse)
			{
				buttonBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
				buttonBrowse.Location = new Point(256, 19);
				buttonBrowse.Name = "browse";
				buttonBrowse.Size = new Size(24, 17);
				dxfTextBox -= 24.0f;
				buttonBrowse.TabIndex = 1;
				buttonBrowse.Text = "...";
				buttonBrowse.Click += new EventHandler(HandleBrowse);
			}

			float dxfAdjustLabel = 0.0f;
			float dyfAdjustLabel = 0.0f;
			float xfLabel = 0.0f;
			float yfLabel = 0.0f;

			this.m_lbl.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
			this.m_lbl.Name = "m_lbl";
			this.m_lbl.TabIndex = 0;
			this.m_lbl.Text = "";
			this.m_lbl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);

			if (m_sLabel != null)
			{
				Graphics gr = CreateGraphics();

				// let's see if the prompt will easily fit before the textbox
				SizeF szLabel = gr.MeasureString(m_sLabel, m_lbl.Font, (int)dxfTextBox);

				if (szLabel.Width > dxfTextBox / 4.0f)
				{
					yfLabel = szLabel.Height;
					dyfAdjustLabel = yfLabel + 12.0f;

					xfLabel = dxfTextBox;
				}
				else
				{
					dxfAdjustLabel = szLabel.Width + 8.0f;
					xfLabel = dxfAdjustLabel;
				}
				this.m_lbl.Text = m_sLabel;
			}

			this.m_lbl.Size = new System.Drawing.Size((int)xfLabel, Math.Max(12, (int)yfLabel));
			this.m_lbl.Location = new System.Drawing.Point(16, 18);

			//
			// textBox1
			//
			this.textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
			this.textBox1.Location = new System.Drawing.Point(16 + (int)dxfAdjustLabel, 16 + (int)dyfAdjustLabel);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size((int)dxfTextBox - (int)dxfAdjustLabel, 20);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);

			// 
			// button1
			// 
			this.button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.button1.Location = new System.Drawing.Point(182, 48 + (int)dyfAdjustLabel);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(48, 24);
			this.button1.TabIndex = 2;
			this.button1.Text = "OK";
			this.button1.Click += new System.EventHandler(this.HandleOK);
			// 
			// button2
			// 
			this.button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.button2.Location = new System.Drawing.Point(232, 48 + (int)dyfAdjustLabel);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(48, 24);
			this.button2.TabIndex = 3;
			this.button2.Text = "Cancel";
			this.button2.Click += new System.EventHandler(this.HandleCancel);

			//
			// InputBox
			//
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 93 + (int)dyfAdjustLabel);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		 this.button2,
																		 this.button1,
																		 this.m_lbl});
			if (fShowBrowse)
			{
				Controls.Add(buttonBrowse);
			}

			if (!fHideInput)
			{
				Controls.Add(this.textBox1);
			}
			//			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "InputBox";
			this.Text = "InputBox";
			this.ResumeLayout(false);

		}

		/* T E X T  B O X  1  _ K E Y  D O W N */
		/*----------------------------------------------------------------------------
			%%Function: textBox1_KeyDown
			%%Qualified: UI.InputBox.textBox1_KeyDown
			%%Contact: rlittle

		----------------------------------------------------------------------------*/
		private void textBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				m_fCanceled = false;
				this.Close();
			}
		}

		private void HandleOK(object sender, System.EventArgs e)
		{
			m_fCanceled = false;
			this.Close();
		}

		private void HandleCancel(object sender, System.EventArgs e)
		{
			m_fCanceled = true;
			this.Close();
		}


		private void HandleBrowse(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();

			if (textBox1.Text != null && textBox1.Text != "")
			{
				string s = Path.GetDirectoryName(textBox1.Text);
				ofd.InitialDirectory = s == null ? "" : s;
			}
			ofd.Filter = m_sFilter;

			if (ofd.ShowDialog() == DialogResult.OK)
				textBox1.Text = ofd.FileName;
		}

		/* S H O W  I N P U T  B O X */
		/*----------------------------------------------------------------------------
			%%Function: ShowInputBox
			%%Qualified: UI.InputBox.ShowInputBox
			%%Contact: rlittle

		----------------------------------------------------------------------------*/
		public static bool ShowInputBox(string sPrompt, out string sResponse)
		{
			return ShowInputBox(sPrompt, null, out sResponse);
		}

		/* S H O W  I N P U T  B O X */
		/*----------------------------------------------------------------------------
			%%Function: ShowInputBox
			%%Qualified: UI.InputBox.ShowInputBox
			%%Contact: rlittle

		----------------------------------------------------------------------------*/
		public static bool ShowInputBox(string sPrompt, string s, out string sResponse)
		{
			InputBox box = new InputBox(sPrompt, s, false, false, null);
			box.m_fCanceled = false;

			box.ShowDialog();
			sResponse = box.textBox1.Text;
			return !box.m_fCanceled;
		}

		public static bool ShowInputBox(string sPrompt, string sLabel, string s, out string sResponse)
		{
			InputBox box = new InputBox(sPrompt, s, false, false, sLabel);
			box.m_fCanceled = false;

			box.ShowDialog();
			sResponse = box.textBox1.Text;
			return !box.m_fCanceled;
		}

		public static bool ShowInputBoxModelessWait(string sPrompt, string sLabel, string s, out string sResponse)
		{
			InputBox box = new InputBox(sPrompt, s, false, true, sLabel);
			box.m_fCanceled = false;

			box.Show();

			// now wait for it to be dismissed
			while (box.Visible)
			{
				Application.DoEvents();
				Thread.Sleep(500);
				Application.DoEvents();
			}

			sResponse = box.textBox1.Text;
			return !box.m_fCanceled;
		}

		public static bool ShowBrowseBox(string sPrompt, string s, out string sResponse, string sFilter, int width)
		{
			InputBox box = new InputBox(sPrompt, s, true, false, null);
			box.Size = new Size(width, box.Size.Height);
			box.m_fCanceled = false;
			box.m_sFilter = sFilter;

			box.m_lbl.Visible = false;
			box.ShowDialog();
			sResponse = box.textBox1.Text;
			return !box.m_fCanceled;
		}
	}

	public class ListViewEx : ListView // flicker free?
	{
		#region Static Functionality

		private static FieldInfo _internalVirtualListSizeField;

		public ListViewEx()
		{
			this.DoubleBuffered = true;
		}
		static ListViewEx()
		{
			_internalVirtualListSizeField = typeof(ListView).GetField("virtualListSize", System.Reflection.BindingFlags.NonPublic | BindingFlags.Instance);

			if (_internalVirtualListSizeField == null)
			{
				string msg =
					"Private field virtualListSize in type System.Windows.Forms.ListView is not found. Workaround is incompatible with installed .NET Framework version, running without workaround.";
				Trace.WriteLine(msg);
			}
		}

		#endregion


		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

		internal IntPtr SendMessage(int msg, IntPtr wparam, IntPtr lparam)
		{
			return SendMessage(new HandleRef(this, this.Handle), msg, wparam, lparam);
		}

		public void SetVirtualListSize(int size)
		{
			// if workaround incompatible with current framework version (usually MONO)
			if (_internalVirtualListSizeField == null)
			{
				VirtualListSize = size;
			}
			else
			{
				if (size < 0)
				{
					throw new ArgumentException("ListViewVirtualListSizeInvalidArgument");
				}

				_internalVirtualListSizeField.SetValue(this, size);
				if ((base.IsHandleCreated && this.VirtualMode) && !base.DesignMode)
				{
					SendMessage(0x102f, new IntPtr(size), new IntPtr(2));
				}
			}
		}
	}

	public class RenderSupp
	{
		/* R E N D E R  H E A D I N G  L I N E */
		/*----------------------------------------------------------------------------
        	%%Function: RenderHeadingLine
        	%%Qualified: TCore.UI.RenderSupp.RenderHeadingLine
        	%%Contact: rlittle
        	
        ----------------------------------------------------------------------------*/
		static public void RenderHeadingLine(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Label lbl = (Label)sender;
			string s = (string)lbl.Tag;

			SizeF sf = e.Graphics.MeasureString(s, lbl.Font);
			int nWidth = (int)sf.Width;
			int nHeight = (int)sf.Height;

			e.Graphics.DrawString(s, lbl.Font, new SolidBrush(Color.SlateBlue), 0, 0);// new System.Drawing.Point(0, (lbl.Width - nWidth) / 2));
			e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Gray), 1), 6 + nWidth + 1, (nHeight / 2), lbl.Width, (nHeight / 2));
		}

	}
}
