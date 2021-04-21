using System.Collections.Generic;

namespace BibLib.Models.ViewModels
{
    public static class PathDescriptorViewModel
    {
        public static List<Anchor> Anchors(string path)
        {
            return null;
        }
    }

    public class Anchor
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Text { get; set; }
    }
}