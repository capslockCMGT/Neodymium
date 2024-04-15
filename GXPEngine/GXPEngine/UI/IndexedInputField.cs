using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.UI
{
    public class IndexedInputField : InputField
    {
        public delegate void IndexedTextUpdatedNamed(int index, string newText);
        public IndexedTextUpdatedNamed IndexedOnTextChanged = null;
        int index;
        public IndexedInputField(int index, int width, int height, int x, int y, string inputDefault, int fontsize = 15) : base(width, height, false, x, y, fontsize)
        {
            this.index = index;
            OnTextChanged += OnClicked;
            message = inputDefault;
        }

        void OnClicked(string text)
        {
            IndexedOnTextChanged?.Invoke(index, text);
        }
    }
}
