namespace ChilliScreenSaver
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using System.Windows.Forms;
    using System.IO;


    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ScreenSaverWindow());
        }
    }
    public partial class ScreenSaverWindow : Form
    {
        // �\������摜��؂�ւ���Ԋu���w�肵�܂��B�P�ʂ̓~���b�B
        private const int ChangingPictureInterval = 2000;

        // �\���������摜�Q���܂ރt�H���_���w�肵�܂��B
        private readonly string TargetDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        // �X�N���[���Z�[�o�[�N�����̃}�E�X�|�C���^�̈ʒu���L�����Ă���
        private Point cursorPoint = Cursor.Position;

        PictureBox pictureBox = new PictureBox();
        Timer Timer1 = new Timer();

        Random rand = new Random();
        List<string> picFiles = new List<string>();

        // �R���X�g���N�^
        public ScreenSaverWindow()
        {
            this.SuspendLayout();

            // �t���X�N���[����
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Minimized;
            this.Controls.Add(pictureBox);
            this.Capture();

            // �s�N�`���[�{�b�N�X�������傫���ɂ���
            pictureBox.Dock = DockStyle.Fill;
            //            pictureBox.SizeMode = PictureBoxSizeMode.Normal;
            pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            //            pictureBox.BackColor = Color.Black;

            this.ResumeLayout(false);

            // �}�E�X����������L�[�������ꂽ��I��
            pictureBox.MouseMove += PictureBox_MouseMove;
            this.KeyDown += ScreenSaverWindow_KeyDown;

            // �\������摜�̃t�@�C���p�X���擾����
            picFiles.AddRange(Directory.GetFiles(TargetDirectory, "*.jpg", SearchOption.AllDirectories));
            picFiles.AddRange(Directory.GetFiles(TargetDirectory, "*.bmp", SearchOption.AllDirectories));
            picFiles.AddRange(Directory.GetFiles(TargetDirectory, "*.png", SearchOption.AllDirectories));

            this.WindowState = FormWindowState.Maximized;
            pictureBox.Image = this.bg;
            // �摜��؂�ւ��邽�߂̃^�C�}�[
            Timer1.Tick += _timer_Tick;
            Timer1.Interval = ChangingPictureInterval;
            Timer1.Start();

            this.Load += ScreenSaverWindow_Load;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            //            ShowPicture();
            blight();
        }

        int oldIndex = -1;
        void ShowPicture()
        {
            // �摜�t�@�C�������݂��Ȃ��ꍇ�͂Ȃɂ��\�����Ȃ��B
            if (picFiles.Count == 0)
                return;

            // �摜�t�@�C�����ЂƂ������݂��Ȃ��ꍇ�͂��ꂾ����\����������B
            if (picFiles.Count == 1)
            {
                if (oldIndex == -1)
                {
                    Image image = Image.FromFile(picFiles[0]);
                    if (image.Width > pictureBox.Width || image.Height > pictureBox.Height)
                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    else
                        pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                    pictureBox.Image = image;
                    return;
                }
                return;
            }

            // �摜�t�@�C�����������݂���ꍇ
            int index = rand.Next(picFiles.Count);

            // �������̂��擾���Ă��܂����ꍇ�͈Ⴄ���̂��擾����܂ł�蒼��
            if (oldIndex == index)
            {
                while (true)
                {
                    int index2 = rand.Next(picFiles.Count);
                    if (oldIndex != index2)
                    {
                        index = index2;
                        break;
                    }
                }
            }

            oldIndex = index;

            Image newImage = Image.FromFile(picFiles[index]);
            Image oldImage = pictureBox.Image;
            if (newImage.Width > pictureBox.Width || newImage.Height > pictureBox.Height)
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            else
                pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            Capture();
//            pictureBox.Image = newImage;
            if (oldImage != null)
                oldImage.Dispose();
        }
        new void Capture()
        {
            // �v���C�}���X�N���[���S��
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            // ��ʑS�̂��R�s�[����
            graphics.CopyFromScreen(new Point(0, 0), new Point(0, 0), bitmap.Size);
            // �O���t�B�b�N�X�̉��
            graphics.Dispose();


            //�\��
            this.bg = bitmap;
//            pictureBox.Image = this.bg;
//            this.Opacity = 1.0;

        }
        Bitmap bg;
        void blight()
        {

        }

        private void ScreenSaverWindow_Load(object sender, EventArgs e)
        {
//            ShowPicture();
            Capture();
        }

        private void ScreenSaverWindow_KeyDown(object sender, KeyEventArgs e)
        {
            Exit();
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Location != this.cursorPoint)
            {
                Exit();
            }
        }

        void Exit()
        {
            Timer1.Dispose();
            Environment.Exit(0);
        }

    }
}