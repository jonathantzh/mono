//
// System.Drawing.Imaging.Metafile.cs
//
// Authors:
//	Christian Meyer, eMail: Christian.Meyer@cs.tum.edu
//	Dennis Hayes (dennish@raytek.com)
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// (C) 2002 Ximian, Inc.  http://www.ximian.com
// Copyright (C) 2004,2006-2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging {

	[MonoTODO ("Metafiles, both WMF and EMF formats, are only partially supported.")]
	[Serializable]
#if ONLY_1_1
	[ComVisible (false)]
#endif
	[Editor ("System.Drawing.Design.MetafileEditor, " + Consts.AssemblySystem_Drawing_Design, typeof (System.Drawing.Design.UITypeEditor))]
	public sealed class Metafile : Image {

		// constructors

		internal Metafile (IntPtr ptr)
		{
			nativeObject = ptr;
		}

		public Metafile (Stream stream) 
		{
			if (stream == null)
				throw new ArgumentException ("stream");

			Status status;
			if (GDIPlus.RunningOnUnix ()) {
				// With libgdiplus we use a custom API for this, because there's no easy way
				// to get the Stream down to libgdiplus. So, we wrap the stream with a set of delegates.
				GDIPlus.GdiPlusStreamHelper sh = new GDIPlus.GdiPlusStreamHelper (stream);
				status = GDIPlus.GdipCreateMetafileFromDelegate_linux (sh.GetHeaderDelegate, sh.GetBytesDelegate, 
					sh.PutBytesDelegate, sh.SeekDelegate, sh.CloseDelegate, sh.SizeDelegate, out nativeObject);
			} else {
				status = GDIPlus.GdipCreateMetafileFromStream (new ComIStreamWrapper (stream), out nativeObject);
			}
			GDIPlus.CheckStatus (status);
		}

		public Metafile (string filename) 
		{
			if (filename == null)
				throw new ArgumentNullException ("filename");
			if (filename.Length == 0)
				throw new ArgumentException ("filename");

			Status status = GDIPlus.GdipCreateMetafileFromFile (filename, out nativeObject);
			if (status == Status.GenericError)
				throw new ExternalException ("Couldn't load specified file.");
			GDIPlus.CheckStatus (status);
		}

		public Metafile (IntPtr henhmetafile, bool deleteEmf) 
		{
			Status status = GDIPlus.GdipCreateMetafileFromEmf (henhmetafile, deleteEmf, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (IntPtr referenceHdc, EmfType emfType) 
		{
			RectangleF rect = new RectangleF ();
			Status status = GDIPlus.GdipRecordMetafile (referenceHdc, emfType, ref rect, 
				MetafileFrameUnit.GdiCompatible, null, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (IntPtr referenceHdc, Rectangle frameRect) 
		{
			Status status = GDIPlus.GdipRecordMetafileI (referenceHdc, EmfType.EmfPlusDual, ref frameRect, 
				MetafileFrameUnit.GdiCompatible, null, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (IntPtr referenceHdc, RectangleF frameRect) 
		{
			Status status = GDIPlus.GdipRecordMetafile (referenceHdc, EmfType.EmfPlusDual, ref frameRect, 
				MetafileFrameUnit.GdiCompatible, null, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader) 
		{
			Status status = GDIPlus.GdipCreateMetafileFromEmf (hmetafile, false, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (Stream stream, IntPtr referenceHdc) 
		{
			RectangleF rect = new RectangleF ();
			Status status = Status.NotImplemented;
			if (GDIPlus.RunningOnUnix ()) {
				// TODO
			} else {
				status = GDIPlus.GdipRecordMetafileStream (new ComIStreamWrapper (stream), referenceHdc, 
					EmfType.EmfPlusDual, ref rect, MetafileFrameUnit.GdiCompatible, null, out nativeObject);
			}
			GDIPlus.CheckStatus (status);
		}

		public Metafile (string fileName, IntPtr referenceHdc) 
		{
			RectangleF rect = new RectangleF ();
			Status status = GDIPlus.GdipRecordMetafileFileName (fileName, referenceHdc, EmfType.EmfPlusDual, ref rect, 
				MetafileFrameUnit.GdiCompatible, null, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (IntPtr referenceHdc, EmfType emfType, string description) 
		{
			RectangleF rect = new RectangleF ();
			Status status = GDIPlus.GdipRecordMetafile (referenceHdc, emfType, ref rect, 
				MetafileFrameUnit.GdiCompatible, description, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit) 
		{
			Status status = GDIPlus.GdipRecordMetafileI (referenceHdc, EmfType.EmfPlusDual, ref frameRect, frameUnit,
				null, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit) 
		{
			Status status = GDIPlus.GdipRecordMetafile (referenceHdc, EmfType.EmfPlusDual, ref frameRect, frameUnit,
				null, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader, bool deleteWmf) 
		{
			Status status = GDIPlus.GdipCreateMetafileFromEmf (hmetafile, deleteWmf, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (Stream stream, IntPtr referenceHdc, EmfType type) 
		{
			RectangleF rect = new RectangleF ();
			Status status = Status.NotImplemented;
			if (GDIPlus.RunningOnUnix ()) {
				// TODO
			} else {
				status = GDIPlus.GdipRecordMetafileStream (new ComIStreamWrapper (stream), referenceHdc, 
					type, ref rect, MetafileFrameUnit.GdiCompatible, null, out nativeObject);
			}
			GDIPlus.CheckStatus (status);
		}

		public Metafile (Stream stream, IntPtr referenceHdc, Rectangle frameRect) 
		{
			Status status = Status.NotImplemented;
			if (GDIPlus.RunningOnUnix ()) {
				// TODO
			} else {
				status = GDIPlus.GdipRecordMetafileStreamI (new ComIStreamWrapper (stream), referenceHdc, 
					EmfType.EmfPlusDual, ref frameRect, MetafileFrameUnit.GdiCompatible, null, 
					out nativeObject);
			}
			GDIPlus.CheckStatus (status);
		}

		public Metafile (Stream stream, IntPtr referenceHdc, RectangleF frameRect) 
		{
			Status status = Status.NotImplemented;
			if (GDIPlus.RunningOnUnix ()) {
				// TODO
			} else {
				status = GDIPlus.GdipRecordMetafileStream (new ComIStreamWrapper (stream), referenceHdc, 
					EmfType.EmfPlusDual, ref frameRect, MetafileFrameUnit.GdiCompatible, null, 
					out nativeObject);
			}
			GDIPlus.CheckStatus (status);
		}

		public Metafile (string fileName, IntPtr referenceHdc, EmfType type) 
		{
			RectangleF rect = new RectangleF ();
			Status status = GDIPlus.GdipRecordMetafileFileName (fileName, referenceHdc, type, ref rect, 
				MetafileFrameUnit.GdiCompatible, null, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (string fileName, IntPtr referenceHdc, Rectangle frameRect) 
		{
			Status status = GDIPlus.GdipRecordMetafileFileNameI (fileName, referenceHdc, EmfType.EmfPlusDual, 
				ref frameRect, MetafileFrameUnit.GdiCompatible, null, out nativeObject);
			GDIPlus.CheckStatus (status);
		}
		
		public Metafile (string fileName, IntPtr referenceHdc, RectangleF frameRect) 
		{
			Status status = GDIPlus.GdipRecordMetafileFileName (fileName, referenceHdc, EmfType.EmfPlusDual, 
				ref frameRect, MetafileFrameUnit.GdiCompatible, null, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type) 
		{
			Status status = GDIPlus.GdipRecordMetafileI (referenceHdc, type, ref frameRect, frameUnit,
				null, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type) 
		{
			Status status = GDIPlus.GdipRecordMetafile (referenceHdc, type, ref frameRect, frameUnit,
				null, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (Stream stream, IntPtr referenceHdc, EmfType type, string description) 
		{
			Status status = Status.NotImplemented;
			if (GDIPlus.RunningOnUnix ()) {
				// TODO
			} else {
				RectangleF rect = new RectangleF ();
				status = GDIPlus.GdipRecordMetafileStream (new ComIStreamWrapper (stream), referenceHdc, 
					type, ref rect, MetafileFrameUnit.GdiCompatible, description, out nativeObject);
			}
			GDIPlus.CheckStatus (status);
		}

		public Metafile (Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit) 
		{
			Status status = Status.NotImplemented;
			if (GDIPlus.RunningOnUnix ()) {
				// TODO
			} else {
				status = GDIPlus.GdipRecordMetafileStreamI (new ComIStreamWrapper (stream), referenceHdc, 
					EmfType.EmfPlusDual, ref frameRect, MetafileFrameUnit.GdiCompatible, null, 
					out nativeObject);
			}
			GDIPlus.CheckStatus (status);
		}

		public Metafile (Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit) 
		{
			Status status = Status.NotImplemented;
			if (GDIPlus.RunningOnUnix ()) {
				// TODO
			} else {
				status = GDIPlus.GdipRecordMetafileStream (new ComIStreamWrapper (stream), referenceHdc, 
					EmfType.EmfPlusDual, ref frameRect, MetafileFrameUnit.GdiCompatible, null, 
					out nativeObject);
			}
			GDIPlus.CheckStatus (status);
		}

		public Metafile (string fileName, IntPtr referenceHdc, EmfType type, string description)
		{
			RectangleF rect = new RectangleF ();
			Status status = GDIPlus.GdipRecordMetafileFileName (fileName, referenceHdc, type, ref rect, 
				MetafileFrameUnit.GdiCompatible, description, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit) 
		{
			Status status = GDIPlus.GdipRecordMetafileFileNameI (fileName, referenceHdc, EmfType.EmfPlusDual, 
				ref frameRect, frameUnit, null, out nativeObject);
			GDIPlus.CheckStatus (status);
		}
		
		public Metafile (string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit)
		{
			Status status = GDIPlus.GdipRecordMetafileFileName (fileName, referenceHdc, EmfType.EmfPlusDual, 
				ref frameRect, frameUnit, null, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type,
			string description) 
		{
			Status status = GDIPlus.GdipRecordMetafileI (referenceHdc, type, ref frameRect, frameUnit,
				description, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type,
			string description) 
		{
			Status status = GDIPlus.GdipRecordMetafile (referenceHdc, type, ref frameRect, frameUnit,
				description, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit,
			EmfType type) 
		{
			Status status = Status.NotImplemented;
			if (GDIPlus.RunningOnUnix ()) {
				// TODO
			} else {
				status = GDIPlus.GdipRecordMetafileStreamI (new ComIStreamWrapper (stream), referenceHdc, 
					type, ref frameRect, frameUnit, null, out nativeObject);
			}
			GDIPlus.CheckStatus (status);
		}

		public Metafile (Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit,
			EmfType type) 
		{
			Status status = Status.NotImplemented;
			if (GDIPlus.RunningOnUnix ()) {
				// TODO
			} else {
				status = GDIPlus.GdipRecordMetafileStream (new ComIStreamWrapper (stream), referenceHdc, 
					type, ref frameRect, frameUnit, null, out nativeObject);
			}
			GDIPlus.CheckStatus (status);
		}

		public Metafile (string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit,
			EmfType type) 
		{
			Status status = GDIPlus.GdipRecordMetafileFileNameI (fileName, referenceHdc, type, ref frameRect, frameUnit, 
				null, out nativeObject);
			GDIPlus.CheckStatus (status);
		}
		
		public Metafile (string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit,
			string description) 
		{
			Status status = GDIPlus.GdipRecordMetafileFileNameI (fileName, referenceHdc, EmfType.EmfPlusDual, 
				ref frameRect, frameUnit, null, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		public Metafile (string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit,
			EmfType type) 
		{
			Status status = GDIPlus.GdipRecordMetafileFileName (fileName, referenceHdc, type, ref frameRect, frameUnit, 
				null, out nativeObject);
			GDIPlus.CheckStatus (status);
		}
		
		public Metafile (string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, 
			string description) 
		{
			Status status = GDIPlus.GdipRecordMetafileFileName (fileName, referenceHdc, EmfType.EmfPlusDual, 
				ref frameRect, frameUnit, description, out nativeObject);
			GDIPlus.CheckStatus (status);
		}
		
		public Metafile (Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, 
			EmfType type, string description) 
		{
			Status status = Status.NotImplemented;
			if (GDIPlus.RunningOnUnix ()) {
				// TODO
			} else {
				status = GDIPlus.GdipRecordMetafileStreamI (new ComIStreamWrapper (stream), referenceHdc, 
					type, ref frameRect, frameUnit, description, out nativeObject);
			}
			GDIPlus.CheckStatus (status);
		}

		public Metafile (Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, 
			EmfType type, string description) 
		{
			Status status = Status.NotImplemented;
			if (GDIPlus.RunningOnUnix ()) {
				// TODO
			} else {
				status = GDIPlus.GdipRecordMetafileStream (new ComIStreamWrapper (stream), referenceHdc, 
					type, ref frameRect, frameUnit, description, out nativeObject);
			}
			GDIPlus.CheckStatus (status);
		}

		public Metafile (string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, 
			EmfType type, string description) 
		{
			Status status = GDIPlus.GdipRecordMetafileFileNameI (fileName, referenceHdc, type, ref frameRect, 
				frameUnit, description, out nativeObject);
			GDIPlus.CheckStatus (status);
		}
		
		public Metafile (string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit,
			EmfType type, string description) 
		{
			Status status = GDIPlus.GdipRecordMetafileFileName (fileName, referenceHdc, type, ref frameRect, frameUnit, 
				description, out nativeObject);
			GDIPlus.CheckStatus (status);
		}

		// methods

		public IntPtr GetHenhmetafile ()
		{
			return nativeObject;
		}

		[MonoLimitation ("Metafiles aren't only partially supported by libgdiplus.")]
		public MetafileHeader GetMetafileHeader ()
		{
			IntPtr header = Marshal.AllocHGlobal (Marshal.SizeOf (typeof (MetafileHeader)));
			try {
				Status status = GDIPlus.GdipGetMetafileHeaderFromMetafile (nativeObject, header);
				GDIPlus.CheckStatus (status);
				return new MetafileHeader (header);
			}
			finally {
				Marshal.FreeHGlobal (header);
			}
		}

		[MonoLimitation ("Metafiles aren't only partially supported by libgdiplus.")]
		public static MetafileHeader GetMetafileHeader (IntPtr henhmetafile)
		{
			IntPtr header = Marshal.AllocHGlobal (Marshal.SizeOf (typeof (MetafileHeader)));
			try {
				Status status = GDIPlus.GdipGetMetafileHeaderFromEmf (henhmetafile, header);
				GDIPlus.CheckStatus (status);
				return new MetafileHeader (header);
			}
			finally {
				Marshal.FreeHGlobal (header);
			}
		}

		[MonoLimitation ("Metafiles aren't only partially supported by libgdiplus.")]
		public static MetafileHeader GetMetafileHeader (Stream stream)
		{
			if (stream == null)
				throw new NullReferenceException ("stream");

			IntPtr header = Marshal.AllocHGlobal (Marshal.SizeOf (typeof (MetafileHeader)));
			try {
				Status status;

				if (GDIPlus.RunningOnUnix ()) {
					// With libgdiplus we use a custom API for this, because there's no easy way
					// to get the Stream down to libgdiplus. So, we wrap the stream with a set of delegates.
					GDIPlus.GdiPlusStreamHelper sh = new GDIPlus.GdiPlusStreamHelper (stream);
					status = GDIPlus.GdipGetMetafileHeaderFromDelegate_linux (sh.GetHeaderDelegate, 
						sh.GetBytesDelegate, sh.PutBytesDelegate, sh.SeekDelegate, sh.CloseDelegate, 
						sh.SizeDelegate, header);
				} else {
					status = GDIPlus.GdipGetMetafileHeaderFromStream (new ComIStreamWrapper (stream), header);
				}
				GDIPlus.CheckStatus (status);
				return new MetafileHeader (header);
			}
			finally {
				Marshal.FreeHGlobal (header);
			}
		}

		[MonoLimitation ("Metafiles aren't only partially supported by libgdiplus.")]
		public static MetafileHeader GetMetafileHeader (string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException ("fileName");

			IntPtr header = Marshal.AllocHGlobal (Marshal.SizeOf (typeof (MetafileHeader)));
			try {
				Status status = GDIPlus.GdipGetMetafileHeaderFromFile (fileName, header);
				GDIPlus.CheckStatus (status);
				return new MetafileHeader (header);
			}
			finally {
				Marshal.FreeHGlobal (header);
			}
		}

		[MonoLimitation ("Metafiles aren't only partially supported by libgdiplus.")]
		public static MetafileHeader GetMetafileHeader (IntPtr henhmetafile, WmfPlaceableFileHeader wmfHeader)
		{
			IntPtr header = Marshal.AllocHGlobal (Marshal.SizeOf (typeof (MetafileHeader)));
			try {
				Status status = GDIPlus.GdipGetMetafileHeaderFromEmf (henhmetafile, header);
				GDIPlus.CheckStatus (status);
				return new MetafileHeader (header);
			}
			finally {
				Marshal.FreeHGlobal (header);
			}
		}

		[MonoLimitation ("Metafiles aren't only partially supported by libgdiplus.")]
		public void PlayRecord (EmfPlusRecordType recordType, int flags, int dataSize, byte[] data)
		{
			Status status = GDIPlus.GdipPlayMetafileRecord (nativeObject, recordType, flags, dataSize, data);
			GDIPlus.CheckStatus (status);
		}
	}
}
