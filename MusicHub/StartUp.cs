namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            var query = context.Database;
            Console.WriteLine(query);

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
            //Console.WriteLine(ExportAlbumsInfo(context, 9));
            //Console.WriteLine(ExportSongsAboveDuration(context,4));

        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();
            var albums = context.Albums
                .Where(a => a.ProducerId.HasValue && a.ProducerId == producerId)
                .Select(a => new
                {
                    AlbumName = a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    ProducerName = a.Producer.Name,
                    Songs = a.Songs.Select(s => new
                    {
                        SongName = s.Name,
                        s.Price,
                        Writer = s.Writer.Name
                    })
                    .OrderByDescending(s => s.SongName)
                    .ThenBy(s => s.Writer).ToArray(),
                    AlbumPrice = a.Price
                }).ToArray();

            foreach (var album in albums.OrderByDescending(a => a.AlbumPrice))
            {
                sb.AppendLine($"-AlbumName: {album.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine("-Songs:");
                int songNumber = 0;
                foreach (var song in album.Songs)
                {
                    songNumber++;
                    sb.AppendLine($"---#{songNumber}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.Price:f2}");
                    sb.AppendLine($"---Writer: {song.Writer}");
                }
                sb.AppendLine($"-AlbumPrice: {album.AlbumPrice:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();

            var songs = context.Songs
                .Select(s => new
                {
                    SongName = s.Name,
                    Writer = s.Writer.Name,
                    AlbumProducer = s.Album.Producer.Name,
                    Performers = s.SongPerformers.Where(p => p.SongId == s.Id),
                    s.Duration
                }).ToArray();

            var filteredSongs = songs.Where(x => (int)(x.Duration.TotalSeconds) > duration).ToArray();

            int songCounter = 0;
            foreach (var song in filteredSongs.OrderBy(s => s.SongName).ThenBy(s => s.Writer))
            {
                songCounter++;
                sb.AppendLine($"-Song #{songCounter}");
                sb.AppendLine($"---SongName: {song.SongName}");
                sb.AppendLine($"---Writer: {song.Writer}");

                if (song.Performers != null)
                {
                    foreach (var performer in song.Performers.OrderBy(s => s.Performer.FirstName).ThenBy(s => s.Performer.LastName))
                    {
                        var performerFullName = performer.Performer.FirstName + " " + performer.Performer.LastName;
                        sb.AppendLine($"---Performer: {performerFullName}");
                    }
                }
                sb.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
                sb.AppendLine($"---Duration: {song.Duration}");
            }
            return sb.ToString().TrimEnd();
        }
    }
}
