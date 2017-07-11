using System.Windows.Forms;
using System;
using System.Linq;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections;

namespace Translation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //맨 앞으로
            this.TopMost = true;
            //clipboard 리스너 등록
            AddClipboardFormatListener(this.Handle);
        }

        //_comboFrom, _comboTo초기화 함수
        private void GoogleTranslatorFrm_Load
              (object sender,
               EventArgs e)
        {
            //번역 가능 언어 리스트 넣어주고 초기값을 Korean과 English로줌
            this._comboFrom.Items.AddRange(Translator.Languages.ToArray());
            this._comboTo.Items.AddRange(Translator.Languages.ToArray());
            this._comboFrom.SelectedItem = "English"; 
            this._comboTo.SelectedItem = "Korean";
        }
        
        private void GoogleTranslator(String text)
        {
            // translator 초기화
            Translator t = new Translator();

            this._editTarget.Text = string.Empty;
            this._editTarget.Update();
            //this._translationSpeakUrl = null;/

            // 텍스트 번역 해줌
            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.label3.Text = "Translating...";
                this.label3.Update();
                // t.Translate(복사한 값, from 국가, to 국가)
                this._editTarget.Text = t.Translate(text.Trim(), (string)this._comboFrom.SelectedItem, (string)this._comboTo.SelectedItem);
                if (t.Error == null)
                {
                    this._editTarget.Update();
                    //this._translationSpeakUrl = t.TranslationSpeechUrl;/
                }
                else
                {
                    MessageBox.Show(t.Error.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                this.label3.Text = string.Format("Translated in {0} mSec", (int)t.TranslationTime.TotalMilliseconds);
                this.Cursor = Cursors.Default;
            }
        }
        
        /// <summary>
        /// Places the given window in the system-maintained clipboard format listener list.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AddClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// Removes the given window from the system-maintained clipboard format listener list.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// Sent when the contents of the clipboard have changed.
        /// </summary>
        private const int WM_CLIPBOARDUPDATE = 0x031D;
        
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                _editSourceText.Text += Clipboard.GetText() + "\n";
            
                GoogleTranslator(Clipboard.GetText());

                /*
             
                //IDataObject iData = Clipboard.GetDataObject();// Clipboard's data.
                //Depending on the clipboard's current data format we can process the data differently. 
                //Feel free to add more checks if you want to process more formats. 
     
                if (iData.GetDataPresent(DataFormats.Text))
                {
                    string text = (string)iData.GetData(DataFormats.Text);
                    // do something with it
                }
                else if (iData.GetDataPresent(DataFormats.Bitmap))
                {
                    Bitmap image = (Bitmap)iData.GetData(DataFormats.Bitmap);
                    // do something with it
                }

                */
            }
        }

        //form이 종료될때 Clipboard 리스너 등록 해제
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            RemoveClipboardFormatListener(this.Handle);// Remove our window from the clipboard's format listener list.
        }

        //private string _translationSpeakUrl;/
    }
}
