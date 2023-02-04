namespace ChilliScreenSaver
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using System.Windows.Forms;
    using System.IO;
    using static System.Windows.Forms.DataFormats;


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
        // 表示する画像を切り替える間隔を指定します。単位はミリ秒。
        private const int ChangingPictureInterval = 2000;

        // 表示したい画像群を含むフォルダを指定します。
        private readonly string TargetDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        // スクリーンセーバー起動時のマウスポインタの位置を記憶しておく
        private Point cursorPoint = Cursor.Position;

        PictureBox pictureBox = new PictureBox();
        Timer Timer1 = new Timer();

        Random rand = new Random();
        List<string> picFiles = new List<string>();

        // コンストラクタ
        public ScreenSaverWindow()
        {
            this.SuspendLayout();

            // フルスクリーン化
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Minimized;
            this.Controls.Add(pictureBox);
            this.Capture();

            // ピクチャーボックスも同じ大きさにする
            pictureBox.Dock = DockStyle.Fill;
            //            pictureBox.SizeMode = PictureBoxSizeMode.Normal;
            pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            //            pictureBox.BackColor = Color.Black;

            this.ResumeLayout(false);

            // マウスが動いたりキーが押されたら終了
            pictureBox.MouseMove += PictureBox_MouseMove;
            this.KeyDown += ScreenSaverWindow_KeyDown;

            // 表示する画像のファイルパスを取得する
            picFiles.AddRange(Directory.GetFiles(TargetDirectory, "*.jpg", SearchOption.AllDirectories));
            picFiles.AddRange(Directory.GetFiles(TargetDirectory, "*.bmp", SearchOption.AllDirectories));
            picFiles.AddRange(Directory.GetFiles(TargetDirectory, "*.png", SearchOption.AllDirectories));

            this.WindowState = FormWindowState.Maximized;
            pictureBox.Image = this.bg;
            Grayscale();
            pictureBox.Image = this.bg;
            // 画像を切り替えるためのタイマー
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
            // 画像ファイルが存在しない場合はなにも表示しない。
            if (picFiles.Count == 0)
                return;

            // 画像ファイルがひとつしか存在しない場合はそれだけを表示し続ける。
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

            // 画像ファイルが複数存在する場合
            int index = rand.Next(picFiles.Count);

            // 同じものを取得してしまった場合は違うものを取得するまでやり直す
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
            // プライマリスクリーン全体
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            // 画面全体をコピーする
            graphics.CopyFromScreen(new Point(0, 0), new Point(0, 0), bitmap.Size);
            // グラフィックスの解放
            graphics.Dispose();
            Console.WriteLine("capture success");

            this.bg = bitmap;


            //表示
//            this.bg = bitmap;
//            pictureBox.Image = this.bg;
//            this.Opacity = 1.0;

        }
        private void Grayscale()
        {
            Bitmap bitmap = new Bitmap(this.bg);

            int width, height;
            //追記：元画像のフォーマット
            //                ImageFormat format;
            //１．指定したパスから画像を読み込む
            //画像サイズを取得
            width = bitmap.Width;
            height = bitmap.Height;
            //追記：元画像のフォーマットを保持
            //                    format = img.RawFormat;
            //ピクセルデータを取得
            Bitmap saveImg = new Bitmap(width, height);
            Color[,] pixelData = new Color[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color originalColor = bitmap.GetPixel(x, y);
                    //グレースケール化
                    int brightness = (int)(originalColor.GetBrightness() * 255);
                    Color newColor = Color.FromArgb(originalColor.A, brightness, brightness, brightness);
                    pixelData[x, y] = newColor;
                    saveImg.SetPixel(x, y, newColor);
                }
            }
            this.bg = saveImg;
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