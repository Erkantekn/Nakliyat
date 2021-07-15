using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nakliyat.Models;
using System.IO;
using Nakliyat.Controllers;
using System.Text;

namespace Nakliyat.Areas.Admin.Controllers
{
    public class DinamikSayfalarController : Controller
    {
        nakliyatEntities db = new nakliyatEntities();
        static int hata = -1;
        SitemapGenerator sitemap = new SitemapGenerator();
        TimeSetCS time = new TimeSetCS();
        [Authorize(Roles = "A,B,C")]
        public ActionResult YeniHizmetBolgesiEkle(int? hata)
        {
            //Sayfaya viewmodel kulanmadan ilçelerin listesini aktarmak için kullanıyoruz
            ViewBag.HizmetBolgeleri = db.pagesHizmetEkleme.ToList();



            var model = new pagesHizmetEkleme();
            var modelDefault = db.defaultHizmetSayfasiBilgileri.FirstOrDefault();
            model.keywords = modelDefault.keywords.ToString().Replace("-DATABASEad-", "SAYFA İSMİ");
            model.sayfaAciklamasi = modelDefault.siteAciklamasi.ToString().Replace("-DATABASEad-", "SAYFA İSMİ");
            return View(model);

        }


        /*
         hata = 0 hatasız:
         hata = 1 daha önce aynı isimde eklenmiş sayfa var
         hata = 2 bir hata oluştu
         hata = 105 Düzenleme Başarılı.
         
             */
        [Authorize(Roles = "A,B,C")]
        [HttpPost]
        public ActionResult YeniHizmetBolgesiEkle(pagesHizmetEkleme model, bool? defaultMu)
        {
            //Sayfaya viewmodel kulanmadan ilçelerin listesini aktarmak için kullanıyoruz
            ViewBag.HizmetBolgeleri = db.pagesHizmetEkleme.ToList();




            hata = 0;
            model.sayfaAdıIngilizceHarfli = EnglishConvert(model.goruntulenecekAd);
            //Eğer daha önce bu isimde bir kayıt yapılmamışsa
            if (db.pagesHizmetEkleme.Where(x => x.sayfaAdıIngilizceHarfli == model.sayfaAdıIngilizceHarfli).FirstOrDefault() == null)
            {
                //eğer otomatik içerik doldurulacaksa veya içerik boş geldiyse
                if (defaultMu == true || model.icerik == null)
                    model.icerik = db.defaultHizmetSayfasiBilgileri.FirstOrDefault().icerik.ToString().Replace("-DATABASEad", model.goruntulenecekAd);
                //model düzenlemeleri tamamdır.
                //şimdi modeli databaseye kaydediyoruz ve veritabanındaki urllere ekleme yapıyoruz
                db.pagesHizmetEkleme.Add(model);
                sitemapUrl newSitemapModel = new sitemapUrl();
                newSitemapModel.Yol = "Home/HizmetBolgelerimiz?name=" + model.sayfaAdıIngilizceHarfli;
                newSitemapModel.Lastmod = time.getTime().ToString("dd-MM-yyyy");
                db.sitemapUrl.Add(newSitemapModel);
                db.SaveChanges();
                //Ardından site.mapı yeniliyoruz.
                foreach (var item in db.sitemapUrl.ToList())
                {
                    sitemap.setFillParametres(item.Yol, item.Lastmod);
                }

                sitemap.generate(db.sirketBilgileri.FirstOrDefault().URL_Sitemap_icin, Server.MapPath("~/"));


                //Ardından girilen bilgilere göre öncelikle sayfayı oluşturuyoruz. 
                string yol = Server.MapPath("~/Views/Home/HizmetBolgelerimiz_" + model.sayfaAdıIngilizceHarfli);
                string yazilacak = "";
                yazilacak += "@{\n";
                yazilacak += "ViewBag.Title = \"" + model.goruntulenecekAd + "\";\n";
                yazilacak += "Layout = \"~/Views/Shared/_Layout.cshtml\";\n";
                yazilacak += "ViewBag.Keywords = \"" + model.keywords + "\";\n";
                yazilacak += "ViewBag.Aciklama = \"" + model.sayfaAciklamasi + "\";\n";
                yazilacak += "}\n";
                yazilacak += "<div class=\"w3-about-us\"><div>";
                yazilacak += model.icerik;
                yazilacak += "</div></div>";
                dosyayaYaz(yazilacak, yol);
            }
            else
                hata = 1;

            return RedirectToAction("YeniHizmetBolgesiEkle");
        }
        string EnglishConvert(string tr)
        {
            string en = "";
            for (int i = 0; i < tr.Length; i++)
            {
                switch (tr[i])
                {
                    case 'ı':
                        en = en + 'i';
                        break;
                    case 'İ':
                        en = en + 'I';
                        break;
                    case 'ü':
                        en = en + 'u';
                        break;
                    case 'Ü':
                        en = en + 'U';
                        break;
                    case 'ö':
                        en = en + 'o';
                        break;
                    case 'Ö':
                        en = en + 'O';
                        break;
                    case 'ş':
                        en = en + 's';
                        break;
                    case 'Ş':
                        en = en + 'S';
                        break;
                    case 'ğ':
                        en = en + 'g';
                        break;
                    case 'Ğ':
                        en = en + 'G';
                        break;
                    case 'ç':
                        en = en + 'c';
                        break;
                    case 'Ç':
                        en = en + 'C';
                        break;
                    default:
                        en = en + tr[i];
                        break;

                }
            }
            return en;
        }
        private void dosyayaYaz(string yaz, string yol)
        {
            string dosya_yolu = yol + ".txt";
            if (System.IO.File.Exists(dosya_yolu))
                System.IO.File.Delete(dosya_yolu);

            FileStream fs = new FileStream(dosya_yolu, FileMode.OpenOrCreate, FileAccess.Write);

            StreamWriter sw = new StreamWriter(fs,new System.Text.UTF8Encoding(true));
            sw.WriteLine(yaz);
            sw.Flush();
            sw.Close();
            fs.Close();
            if (System.IO.File.Exists(yol + ".cshtml"))
                System.IO.File.Delete(yol + ".cshtml");
            System.IO.File.Move(yol + ".txt", yol + ".cshtml");
        }

        [Authorize(Roles = "A,B,C")]
        public ActionResult HizmetBolgesiDuzenle(string name)
        {
            //Sayfaya viewmodel kulanmadan ilçelerin listesini aktarmak için kullanıyoruz
            ViewBag.HizmetBolgeleri = db.pagesHizmetEkleme.ToList();



            var model = db.pagesHizmetEkleme.Where(x => x.sayfaAdıIngilizceHarfli == name).FirstOrDefault();
            if (model != null)
                return View(model);
            else
                return HttpNotFound();
        }
        [Authorize(Roles = "A,B,C")]
        [HttpPost]
        public ActionResult HizmetBolgesiDuzenle(pagesHizmetEkleme model)
        {
            hata = 0;
            model.sayfaAdıIngilizceHarfli = EnglishConvert(model.goruntulenecekAd);
            
            var orjModel = db.pagesHizmetEkleme.Where(x => x.sayfaAdıIngilizceHarfli == model.sayfaAdıIngilizceHarfli).FirstOrDefault();
            orjModel.icerik = model.icerik;
            orjModel.sayfaAciklamasi = model.sayfaAciklamasi;
            orjModel.keywords = model.keywords;
            db.SaveChanges();
            hata = 105;
            return RedirectToAction("HizmetBolgesiDuzenle", new { name = model.sayfaAdıIngilizceHarfli.ToString() });
        }
        }
}