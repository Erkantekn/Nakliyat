using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Nakliyat.Models;
using Nakliyat.Controllers;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using Nakliyat.Areas.Admin.ViewModel;

namespace Nakliyat.Areas.Admin.Controllers
{
    [Authorize]
    public class AdminHomeController : Controller
    {
        nakliyatEntities db = new nakliyatEntities();
        TimeSetCS time = new TimeSetCS();

        // GET: Admin/AdminHome
        [Authorize(Roles = "A,B,C,D,E")]
        public ActionResult Index()
        {
            //Sayfaya viewmodel kulanmadan ilçelerin listesini aktarmak için kullanıyoruz
            ViewBag.HizmetBolgeleri = db.pagesHizmetEkleme.ToList();
            


            var model = db.iletisimFormu.ToList();
            ViewBag.Toplam = model.Count();
            ViewBag.Okunan = model.Where(x => x.OkunduMu == true).Count();
            ViewBag.Okunmayan = ViewBag.Toplam - ViewBag.Okunan;
            return View(model);

        }
        [Authorize(Roles = "A,B,C,D,E")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        [HttpPost]
        [Authorize(Roles = "A,B,C,D,E")]
        public ActionResult iletisimFormDetay(int? id)
        {

            //Sayfaya viewmodel kulanmadan ilçelerin listesini aktarmak için kullanıyoruz
            ViewBag.HizmetBolgeleri = db.pagesHizmetEkleme.ToList();




            //gelen id yi Veritabanımızda kontrol ediyoruz ve gelen employee partialviewimize gönderiyoruz
            var model = db.iletisimFormu.FirstOrDefault(x => x.id == id);
            if (model != null)
            {

                return PartialView(model);
            }
            else
                return HttpNotFound();
        }

        [Authorize(Roles = "A,B,C,D,E")]
        public ActionResult Okunmadi(int? id, int? oku)
        {
            var model = db.iletisimFormu.FirstOrDefault(x => x.id == id);
            if (model != null)
            {
                model.OkunduMu = Convert.ToBoolean(oku);
                db.SaveChanges();
            }
            else
                return HttpNotFound();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "A,B,C,D,E")]
        public ActionResult iletisimFormSil(int? id)
        {
            var model = db.iletisimFormu.FirstOrDefault(x => x.id == id);
            if (model != null)
            {
                db.iletisimFormu.Remove(model);
                db.SaveChanges();
            }
            else
                return HttpNotFound();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "A,B,C,D")]
        public ActionResult Galeri(int? hata, int? kategoriId)
        {
            //Sayfaya viewmodel kulanmadan ilçelerin listesini aktarmak için kullanıyoruz
            ViewBag.HizmetBolgeleri = db.pagesHizmetEkleme.ToList();




            ViewBag.Mesaj = hata;
            hata = -1;
            var model = new GaleriViewModel() { ResimlerV = new List<Resimler>() { new Resimler() }, ResimKategorileriV = db.ResimKategorileri.ToList() };
            if (kategoriId != null)
                model = new GaleriViewModel() { ResimlerV = db.Resimler.Where(x => x.KategoriID == kategoriId).ToList(), ResimKategorileriV = db.ResimKategorileri.ToList() };

            List<SelectListItem> KategoriListesi = (from k in db.ResimKategorileri.ToList()
                                                    select new SelectListItem
                                                    {
                                                        Text = k.KategoriAdı,
                                                        Value = k.id.ToString()
                                                    }).ToList();
            ViewBag.Liste = KategoriListesi;
            return View(model);
        }

        [Authorize(Roles = "A,B,C,D")]
        public ActionResult ResimGuncellePartial(int? id)
        {
            //Sayfaya viewmodel kulanmadan ilçelerin listesini aktarmak için kullanıyoruz
            ViewBag.HizmetBolgeleri = db.pagesHizmetEkleme.ToList();




            //gelen id yi Veritabanımızda kontrol ediyoruz ve gelen employee partialviewimize gönderiyoruz
            var model = db.Resimler.Where(x => x.id == id).FirstOrDefault();
            if (model != null)
            {

                return PartialView(model);
            }
            else
                return HttpNotFound();
        }
        static int hata = -1;
        public static Image ResizeImage(Image imgToResize, Size destinationSize)
        {
            var originalWidth = imgToResize.Width;
            var originalHeight = imgToResize.Height;

            //how many units are there to make the original length
            var hRatio = (float)originalHeight / destinationSize.Height;
            var wRatio = (float)originalWidth / destinationSize.Width;

            //get the shorter side
            var ratio = Math.Min(hRatio, wRatio);

            var hScale = Convert.ToInt32(destinationSize.Height * ratio);
            var wScale = Convert.ToInt32(destinationSize.Width * ratio);

            //start cropping from the center
            var startX = (originalWidth - wScale) / 2;
            var startY = (originalHeight - hScale) / 2;

            //crop the image from the specified location and size
            var sourceRectangle = new Rectangle(startX, startY, wScale, hScale);

            //the future size of the image
            var bitmap = new Bitmap(destinationSize.Width, destinationSize.Height);

            //fill-in the whole bitmap
            var destinationRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            //generate the new image
            using (var g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(imgToResize, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);
            }

            return bitmap;

        }


        /*
         hata
         -1 : işlem yapılmadı
         0 : hatasız
         1 : MaxValue Sınırı aşıldı
         2 : resim bulunamadı
         3 : yüklenen dosya bir resim dosyası değil

        10 : resim silindi
        11 : kayıt silindi fakat resim silinemedi
        12 : kayıt bulunamadı
             */
        [Authorize(Roles = "A,B,C,D")]
        public ActionResult ResimSecildi(int? id, HttpPostedFileBase uploadFile, bool yeni, int? kategoriID, string title)
        {
            hata = 0;
            if (uploadFile != null)
            {

                if (uploadFile.IsImage())
                {
                    string resimAdi = System.IO.Path.GetFileName(uploadFile.FileName);
                    string random = Path.GetRandomFileName();
                    string adres = Server.MapPath("~/images/" + random + resimAdi);
                    uploadFile.SaveAs(adres);

                    //Kategoriyi bulup atıyoruz
                    ResimKategorileri kategori = db.ResimKategorileri.Where(x => x.id == kategoriID).FirstOrDefault();


                    if (yeni)
                    {

                        //Eğer kategırideki max value aşılmamışsa
                        if (kategori.MaxValue > db.Resimler.Where(x => x.KategoriID == kategoriID).Count())
                        {
                            Image gecici = Image.FromFile(adres);
                            //Eğer kategoride bulunan yükseklik ve genişlik eşit değilse
                            if (kategori.Yukseklik != gecici.Height || kategori.Genislik != gecici.Width)
                            {

                                Image geciciResize = ResizeImage(gecici, new Size(kategori.Genislik, kategori.Yukseklik));
                                //ilk upload edileni silip üstüne resize edileni kaydediyoruz
                                geciciResize.Save(adres + "yeni");
                                geciciResize.Dispose();
                                gecici.Dispose();
                                System.IO.File.Delete(adres);
                                System.IO.File.Move(adres + "yeni", adres);
                            }
                            else
                            {
                                //geciciyi kapatıyoruz
                                gecici.Dispose();
                            }
                            //Veritabanına eklenen resmi ekliyorz
                            Resimler newResim = new Resimler();
                            newResim.KategoriID = kategori.id;
                            newResim.ResimYolu = "/images/" + random + resimAdi;
                            newResim.ResimTitle = title;
                            db.Resimler.Add(newResim);
                            db.SaveChanges();


                        }
                        else
                        {
                            System.IO.File.Delete(adres);
                            hata = 1;
                        }

                    }
                    //Eğer yeni değilse yani resim değişecekse
                    else
                    {

                        Image gecici = Image.FromFile(adres);
                        //Eğer kategoride bulunan yükseklik ve genişlik eşit değilse
                        if (kategori.Yukseklik != gecici.Height || kategori.Genislik != gecici.Width)
                        {

                            Image geciciResize = ResizeImage(gecici, new Size(kategori.Genislik, kategori.Yukseklik));
                            //ilk upload edileni silip üstüne resize edileni kaydediyoruz
                            geciciResize.Save(adres + "yeni");
                            geciciResize.Dispose();
                            gecici.Dispose();
                            System.IO.File.Delete(adres);
                            System.IO.File.Move(adres + "yeni", adres);

                        }
                        else
                        {
                            //geciciyi kapatıyoruz
                            gecici.Dispose();
                        }
                        //geciciyi kapatıyoruz
                        gecici.Dispose();
                        //Veritabanına eklenen resmin yolunu güncelliyoruz
                        Resimler oldResim = db.Resimler.Where(x => x.id == id).FirstOrDefault();
                        //eski resmi siliyoruz
                        System.IO.File.Delete(Server.MapPath("~" + oldResim.ResimYolu));
                        //veritabanına yenisini kaydediyoruz
                        oldResim.ResimYolu = "/images/" + random + resimAdi;
                        oldResim.ResimTitle = title;
                        db.SaveChanges();






                    }
                }
                else
                    hata = 3;
            }
            //uploadFile null olmasına rağmen title değişmiş mi diye kontrol ediyoruz.
            else
            {
                if (!yeni)
                {
                    var resim = db.Resimler.Where(x => x.id == id).FirstOrDefault();
                    if (resim.ResimTitle != title)
                    {
                        resim.ResimTitle = title;
                        db.SaveChanges();
                    }
                }
                else
                hata = 2;
            }
            return RedirectToAction("Galeri", new { hata });
        }

        [Authorize(Roles = "A,B,C,D")]
        public ActionResult ResimSil(int? id)
        {
            var model = db.Resimler.FirstOrDefault(x => x.id == id);
            hata = 10;
            if (model != null)
            {
                db.Resimler.Remove(model);
                db.SaveChanges();
                try
                {
                    string adres = Server.MapPath("~" + model.ResimYolu);
                    System.IO.File.Delete(adres);
                    
                }
                catch 
                {
                    hata = 11;
                    
                }

            }
            else 
            {
                hata = 12;
                
            }
            return RedirectToAction("Galeri",new { hata });
        }

      
    }
}