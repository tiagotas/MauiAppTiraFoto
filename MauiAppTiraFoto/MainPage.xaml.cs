using SQLite;
using System.Collections.ObjectModel;
using System.IO;

namespace MauiAppTiraFoto
{
    public partial class MainPage : ContentPage
    {

        ObservableCollection<Image> lista = new ObservableCollection<Image>();


        readonly SQLiteAsyncConnection con;

        public MainPage()
        {
            InitializeComponent();

            //getdata();
            var path = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), "sqlite1.db3");

            con = new SQLiteAsyncConnection(path);
            con.CreateTableAsync<Image>().Wait();
        }

        protected async override void OnAppearing()
        {
            try
            {
                lista.Clear();

                List<Image> tmp = con.Table<Image>().ToListAsync().Result;

                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }



        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    // save the file into local storage
                    string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                    Stream sourceStream = await photo.OpenReadAsync();
                    FileStream localFileStream = File.OpenWrite(localFilePath);

                    await sourceStream.CopyToAsync(localFileStream);

                    //foto_tirada.Source = ImageSource.FromFile(localFileStream.Name);
                    //foto_tirada.Source = ImageSource.FromStream(() => sourceStream);


                    var result = con.InsertAsync(new Image
                    {
                        Content = GetImageBytes(sourceStream),
                        FileName = localFileStream.Name
                    });

                    //insertdata(sourceStream, localFileStream);
                }
            }
        }

        private void insertdata(Stream stream, FileStream arquivo)
        {

            var result = con.InsertAsync(new Image
            {
                Content = GetImageBytes(stream),
                FileName = arquivo.Name
            });

            //Console.WriteLine("-----------------------------------------");
            //Console.WriteLine("REGISTROS AFETADOS: ");
            //Console.WriteLine(result.Result);
            //Console.WriteLine("-----------------------------------------");


            var image_recuperada = con.Table<Image>().ToListAsync().Result.FirstOrDefault();

            if (image_recuperada != null)
            {
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("ARQUIVO SALVO: ");
                Console.WriteLine(image_recuperada.Id);
                Console.WriteLine(image_recuperada.FileName);
                Console.WriteLine("-----------------------------------------");

                //byte[] b = image.Content;
                //Stream ms = new MemoryStream(b);

                //foto_tirada.Source = ImageSource.FromStream(() => ms);
            }

            //Console.WriteLine("-----------------------------------------");
            //Console.WriteLine("Linhas Add:");
            //Console.WriteLine(result);
            //Console.WriteLine("-----------------------------------------");

            //
            //foto_tirada.Source = ImageSource.FromFile(arquivo.Name);

            //Console.WriteLine("-----------------------------------------");
            //Console.WriteLine("ID da FOTO: " + result);
            //Console.WriteLine(arquivo.Name);
            //Console.WriteLine("-----------------------------------------");


            //sl.Children.Add(new Label() { Text = result > 0 ? "insert successful insert" : "fail insert" });


            // getdata();
        }

        private void getdata()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sqlite1.db3");
            using (var con = new SQLiteConnection(path))
            {
                var image = con.Query<Image>("SELECT content FROM Image ;").FirstOrDefault();

                if (image != null)
                {

                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine("ARQUIVO SALVO: ");
                    Console.WriteLine(image.Id);
                    Console.WriteLine(image.FileName);
                    Console.WriteLine("-----------------------------------------");

                    byte[] b = image.Content;
                    Stream ms = new MemoryStream(b);

                    foto_tirada.Source = ImageSource.FromStream(() => ms);
                }
            }
        }

        /**
         * Converte um stream da camera para um vetor de bytes
         */
        private byte[] GetImageBytes(Stream stream)
        {
            byte[] ImageBytes;

            using (var memoryStream = new System.IO.MemoryStream())
            {
                stream.CopyTo(memoryStream);

                ImageBytes = memoryStream.ToArray();
            }

            return ImageBytes;
        }


        /**
         * Converte um vetor de bytes para um stream de imagem
         */
        public Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            try
            {
                lista.Clear();

                List<Image> tmp = con.Table<Image>().ToListAsync().Result;

                Console.WriteLine("-------------------------------");
                Console.WriteLine("Registros: " + tmp.Count);
                Console.WriteLine("-------------------------------");

                tmp.ForEach(i =>
                {
                   
                    Console.WriteLine("Arquivo: " + i.FileName);
                    Console.WriteLine("Tamanho do Stream: " + i.Content.LongLength);
                    
                    lista.Add(i);

                    byte[] b = i.Content;
                    Stream ms = new MemoryStream(b);

                    foto_tirada.Source = ImageSource.FromStream(() => ms);
                    //foto_tirada.Source = ImageSource.FromFile(i.FileName);
                });
                
                
                
               
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }

        }
    }

}
