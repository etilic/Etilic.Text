using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etilic.Text
{
    /// <summary>
    /// An interface for classes which store the state of an <see cref="Etilic.Text.IParsable"/>.
    /// </summary>
    public interface IRestorePoint : IDisposable
    {
        #region Restore
        /// <summary>
        /// Restores the underlying <see cref="Etilic.Text.IParsable"/> to the state it was
        /// in when this object was created.
        /// </summary>
        void Restore();
        #endregion
    }
}
