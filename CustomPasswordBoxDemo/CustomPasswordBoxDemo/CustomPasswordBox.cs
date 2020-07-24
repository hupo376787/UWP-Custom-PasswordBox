using System;
using System.Linq;
using System.Security;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CustomPasswordBoxDemo
{
    internal class CustomPasswordBox : TextBox
    {
        #region Member Variables
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(SecureString), typeof(CustomPasswordBox), new PropertyMetadata(new SecureString()));

        public static readonly DependencyProperty RealPasswordProperty =
            DependencyProperty.Register("RealPassword", typeof(string), typeof(CustomPasswordBox), new PropertyMetadata(string.Empty));

        private DispatcherTimer maskTimer;
        #endregion

        #region Constructors
        public CustomPasswordBox()
        {
            PreviewKeyDown += CustomPasswordBox_PreviewKeyDown;
            CharacterReceived += CustomPasswordBox_CharacterReceived;
            BeforeTextChanging += CustomPasswordBox_BeforeTextChanging;
            SelectionChanged += CustomPasswordBox_SelectionChanged;
            ContextMenuOpening += CustomPasswordBox_ContextMenuOpening;
            TextCompositionStarted += CustomPasswordBox_TextCompositionStarted;

            maskTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 2) };
            maskTimer.Tick += MaskTimer_Tick;
        }
        #endregion

        #region Commands & Properties
        /// <summary>
        /// Secure password
        /// </summary>
        public SecureString Password
        {
            get
            {
                return (SecureString)GetValue(PasswordProperty);
            }

            set
            {
                SetValue(PasswordProperty, value);
            }
        }

        /// <summary>
        /// Real password , ie, plain text
        /// </summary>
        public string RealPassword
        {
            get
            {
                return (string)GetValue(RealPasswordProperty);
            }

            set
            {
                SetValue(RealPasswordProperty, value);
            }
        }
        #endregion

        #region Methods
        private void CustomPasswordBox_PreviewKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            switch (e.OriginalKey)
            {
                case VirtualKey.Back:
                case VirtualKey.Delete:
                    if (SelectionLength > 0)
                    {
                        RemoveFromSecureString(SelectionStart, SelectionLength);
                    }
                    else if (e.OriginalKey == VirtualKey.Delete && SelectionStart < Text.Length)
                    {
                        RemoveFromSecureString(SelectionStart, 1);
                    }
                    else if (e.OriginalKey == VirtualKey.Back && SelectionStart > 0)
                    {
                        int caretIndex = SelectionStart;
                        if (SelectionStart > 0 && SelectionStart < Text.Length)
                            caretIndex = caretIndex - 1;
                        RemoveFromSecureString(SelectionStart - 1, 1);
                        //SelectionStart = caretIndex;
                    }

                    e.Handled = true;
                    break;

                default:
                    //e.Handled = true;
                    break;
            }
        }

        /// <summary>
        /// Fired when a character received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CustomPasswordBox_CharacterReceived(UIElement sender, Windows.UI.Xaml.Input.CharacterReceivedRoutedEventArgs args)
        {
            if (!char.IsDigit(args.Character))
                return;

            AddToSecureString(args.Character.ToString());
            args.Handled = true;
        }

        private void CustomPasswordBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c) && !char.ToString(c).ToString().Equals("●"));
            //if (args.NewText.Replace("●", "") != "")
            //    AddToSecureString(args.NewText.Replace("●", ""));
        }

        /// <summary>
        /// Disable Context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomPasswordBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Set TextBox non-selectable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomPasswordBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SelectionStart = Text.Length;
            SelectionLength = 0;
        }

        /// <summary>
        /// Prevent IME(Chinese word) input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CustomPasswordBox_TextCompositionStarted(TextBox sender, TextCompositionStartedEventArgs args)
        {
            return;
        }

        /// <summary>
        /// Time to mask character
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaskTimer_Tick(object sender, object e)
        {
            MaskAllDisplayText();
        }

        /// <summary>
        /// Input character
        /// </summary>
        /// <param name="text"></param>
        private void AddToSecureString(string text)
        {
            if (SelectionLength > 0)
            {
                RemoveFromSecureString(SelectionStart, SelectionLength);
            }

            if (Password.Length >= 4 || RealPassword.Length >= 4)
                return;
            foreach (char c in text)
            {
                System.Diagnostics.Debug.WriteLine(text);
                int caretIndex = SelectionStart;
                if (caretIndex - 1 < 0)
                    Password.InsertAt(0, c);
                else
                    Password.InsertAt(caretIndex - 1, c);
                RealPassword += c.ToString();
                //MaskAllDisplayText();
                if (caretIndex == Text.Length)
                {
                    maskTimer.Stop();
                    maskTimer.Start();
                    //Text = Text.Insert(caretIndex++, c.ToString());
                }
                else
                {
                    //Text = Text.Insert(caretIndex++, "●");
                }
                SelectionStart = Text.Length;
            }
        }

        /// <summary>
        /// Remove character
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="trimLength"></param>
        private void RemoveFromSecureString(int startIndex, int trimLength)
        {
            int caretIndex = SelectionStart;
            for (int i = 0; i < trimLength; ++i)
            {
                Password.RemoveAt(startIndex);
                RealPassword = RealPassword.Remove(startIndex, 1);
            }

            Text = Text.Remove(startIndex, trimLength);
            SelectionStart = caretIndex;
        }

        /// <summary>
        /// Mask Displayed Text
        /// </summary>
        private void MaskAllDisplayText()
        {
            maskTimer.Stop();
            int caretIndex = SelectionStart;
            Text = new string('●', Text.Length);
            SelectionStart = caretIndex;
        }

        #endregion
    }
}
