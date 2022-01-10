using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xceed.Document.NET;
using Xceed.Words.NET;
using CloudLearningAPI.Models;
using CloudLearningAPI.Properties;
using CloudLearningAPI.Interfaces;
using CsvHelper;
using System.Globalization;
using System.Threading.Tasks;

namespace CloudLearningAPI.Services
{
    public class FileService : IFileService
    {

        private Stream _baseDocument;
        private List<LanguageModel> _supportedLanguages;
        private readonly IDropboxService _dropboxService;

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public FileService(IDropboxService dropboxService)
        {
            _supportedLanguages = LoadLanguagesFromJSON();
            _baseDocument = LoadBaseDocument();
            _dropboxService = dropboxService;
        }
        #endregion

        #region Word File generator
        /// <summary>
        /// Generate the Word document which will be sent to the teacher.
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        public async Task<string> GenerateWordDoc(Course course)
        {
            string fullCourseNumber = $"{course.Coursenumber}/{DateTime.Now.ToString("yy")}";
            string fileName = $"{course.Coursenumber}_{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.docx";
            string fullPath = Path.Combine("./WordFiles", fileName);
            if (!Directory.Exists("./WordFiles"))
            {
                Directory.CreateDirectory("./WordFiles");
            }
            using (DocX document = DocX.Load(_baseDocument))
            {
                document.ReplaceText("{coursenumber}", fullCourseNumber);
                var table = document.Tables[0];
                table.Design = TableDesign.TableGrid;
                foreach (Participant participant in course.Participants)
                {
                    var row = table.InsertRow();
                    row.Cells[0].InsertParagraph(participant.Email);
                    row.Cells[1].InsertParagraph(participant.Firstname);
                    row.Cells[2].InsertParagraph(participant.Lastname);
                    row.Cells[3].InsertParagraph("PW4you");
                }
                using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    document.SaveAs(fs);
                }
            }
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(fullPath)))
            {
                var result = await _dropboxService.UploadFile(fullPath, ms);
                if (!result)
                {
                    return null;
                }
            }
            foreach (var file in Directory.EnumerateFiles("./WordFiles"))
            {
                File.Delete(file);
            }
            return fileName;
        }
        #endregion

        #region Import File generator
        public async Task<string> GenerateImportFile(Course course, bool cloudlearning = false, bool homework = false, bool ebook = false)
        {
            string fullCourseNumber = $"{course.Coursenumber}/{DateTime.Now.ToString("yy")}";
            string fileName = $"{course.Coursenumber}_{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.csv";
            string fullPath = Path.Combine("./ImportFiles", fileName);
            string level = course.Level.Split('/')[0];
            if (level == "C2")
                level = "C1";
            string ebookString = null;
            if (!Directory.Exists("./ImportFiles"))
            {
                Directory.CreateDirectory("./ImportFiles");
            }

            LanguageModel? language = _supportedLanguages.Where(language => language.Language == course.Language).FirstOrDefault();
            if (ebook && level is not null) 
            {
                PropertyInfo pinfo = typeof(Level).GetProperty(level);
                ebookString = pinfo.GetValue(language.Levels, null).ToString();
            }
            var importData = course.Participants.Select(participant => new UserImport 
            {
                Username = participant.Email,
                Email = participant.Email,
                Firstname = participant.Firstname,
                Lastname = participant.Lastname,
                Cohort = cloudlearning ? language?.Cloudlearning : null,
                Homework = homework ? $"{course.Coursenumber}/{DateTime.Now.ToString("yy")}" : null,
                Ebook = ebook ? ebookString : null,
                Group = ebook ? course.Coursenumber : null
            }).ToList();
            using (var writer = new StreamWriter(fullPath))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(importData);
                }
            }
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(fullPath)))
            {
                var result = await _dropboxService.UploadFile(Path.Combine("ImportFiles", fileName), ms);
                if (!result)
                {
                    return null;
                }
            }
            foreach (var file in Directory.EnumerateFiles("./ImportFiles"))
            {
                File.Delete(file);
            }
            return fileName;
        }
        #endregion

        #region Helper functions
        /// <summary>
        /// Loads the base Word template
        /// </summary>
        /// <returns></returns>
        private Stream LoadBaseDocument()
        {
            var resourceName = "CloudLearningAPI.Resources.base_template.docx";
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream baseDocument = assembly.GetManifestResourceStream(resourceName);
            if (baseDocument is null) 
            {
                throw new FileNotFoundException("Base Word document is not found!");
            }
            return baseDocument;
        }
        private List<LanguageModel> LoadLanguagesFromJSON()
        {
            using MemoryStream memoryStream = new MemoryStream(Resources.languages);
            using StreamReader file = new StreamReader(memoryStream);
            if (file is null)
            {
                throw new FileNotFoundException("Languages.json file not found!");
            }
            string json = file.ReadToEnd();
            List<LanguageModel> languages = JsonConvert.DeserializeObject<List<LanguageModel>>(json);
            return languages;
        }
        #endregion
    }
}
