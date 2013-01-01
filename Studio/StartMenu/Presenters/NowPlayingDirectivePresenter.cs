namespace Advent.VmcStudio.StartMenu.Presenters
{
    internal class NowPlayingDirectivePresenter
    {
        public string Text { get; private set; }

        public string Directive { get; private set; }

        internal NowPlayingDirectivePresenter(string text, string directive)
        {
            this.Text = text;
            this.Directive = directive;
        }
    }
}
