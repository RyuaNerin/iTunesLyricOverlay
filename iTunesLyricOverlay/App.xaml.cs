using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using iTunesLyricOverlay.Database;
using LiteDB;

namespace iTunesLyricOverlay
{
    public partial class App : Application
    {
        private static LiteDatabase m_database;
        public static CachedCollection<LyricArchive> LyricCollection;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var asm = Assembly.GetExecutingAssembly();

            CrashReport.Init();

            m_database = new LiteDatabase(Path.GetFileNameWithoutExtension(asm.Location) + ".db");
            m_database.Engine.UserVersion = 0;

            Config.Load(m_database);

            LyricCollection = new CachedCollection<LyricArchive>(
                m_database.GetCollection<LyricArchive>("lyrics"),
                le => le.LyricArchiveId);

            foreach (var itemInvaild in LyricCollection.FindAll().Where(le => le.IsInvalid).ToArray())
            {
                Debug.WriteLine($"Invaild : {itemInvaild.LyricArchiveId}");
                LyricCollection.Delete(itemInvaild.LyricArchiveId);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Config.Save();

            m_database.Shrink();
            m_database.Dispose();
        }
    }
}
