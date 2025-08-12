
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

 
namespace TheChosenProject.ServerCore
{
  public static class Translator
  {
    private static readonly string CONFIG_DIR_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ConquerDialogsTranslatorAPI");
    private static readonly string CONFIG_FILE_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ConquerDialogsTranslatorAPI", "Cache.json");
    public static List<Translation> Translations = new List<Translation>();

    public static string GetTranslatedString(
      string _Text,
      Translator.Language _FromLanguage,
      Translator.Language _ToLanguage)
    {
      if (!ServerKernel.Allow_Server_Translate)
        return _Text;
      Translator.LoadCache();
      return Translator.GetStringFromCache(new Translation()
      {
        Key = _Text,
        FromLang = _FromLanguage,
        ToLang = _ToLanguage
      });
    }

    private static void LoadCache()
    {
      if (!Directory.Exists(Translator.CONFIG_DIR_PATH))
        Directory.CreateDirectory(Translator.CONFIG_DIR_PATH);
      if (!System.IO.File.Exists(Translator.CONFIG_FILE_PATH))
        System.IO.File.WriteAllText(Translator.CONFIG_FILE_PATH, JsonConvert.SerializeObject((object) Translator.Translations));
      Translator.Translations = JsonConvert.DeserializeObject<List<Translation>>(System.IO.File.ReadAllText(Translator.CONFIG_FILE_PATH));
    }

    private static void SaveCache()
    {
      if (!System.IO.File.Exists(Translator.CONFIG_FILE_PATH))
        return;
      System.IO.File.WriteAllText(Translator.CONFIG_FILE_PATH, JsonConvert.SerializeObject((object) Translator.Translations));
    }

    private static string GetStringFromCache(Translation translation)
    {
      Translation translation1 = Translator.Translations.Where<Translation>((Func<Translation, bool>) (x => x.Key == translation.Key && x.FromLang == translation.FromLang && x.ToLang == translation.ToLang)).FirstOrDefault<Translation>();
      if (translation1 != null)
        return translation1.Value;
      HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create("https://translate.googleapis.com/translate_a/single?client=gtx&sl=" + translation.FromLang.ToString() + "&tl=" + translation.ToLang.ToString() + "&dt=t&q=" + WebUtility.UrlEncode(translation.Key));
      httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
      string str1 = "";
      using (HttpWebResponse response = (HttpWebResponse) httpWebRequest.GetResponse())
      {
        using (Stream responseStream = response.GetResponseStream())
        {
          using (StreamReader streamReader = new StreamReader(responseStream))
            str1 = streamReader.ReadToEnd();
        }
      }
      string str2 = translation.Key;
      JArray source = JsonConvert.DeserializeObject<JArray>(str1);
      if (source.First<JToken>() != null)
        str2 = "";
      foreach (JToken child in source.First<JToken>().Children())
        str2 += child.First<JToken>().ToString();
      translation.Value = str2;
      Translator.Translations.Add(translation);
      Translator.SaveCache();
      return translation.Value;
    }

    public static void ClearCache()
    {
      Translator.Translations.Clear();
      System.IO.File.WriteAllText(Translator.CONFIG_FILE_PATH, JsonConvert.SerializeObject((object) Translator.Translations));
    }

    public enum Language
    {
      AR,
      EN,
      PT,
      FR,
      DE,
    }
  }
}
