#region Licence...
/*
The MIT License (MIT)

Copyright (c) 2014 Oleg Shilo

Permission is hereby granted, 
free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion

using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using IO = System.IO;

namespace WixSharp
{
    /// <summary>
    /// Defines files of a given source directory to be installed on target system.
    /// <para>
    /// Use this class to define files to be automatically included into the deployment solution
    /// if they name complain with specified wildcard character pattern (<see cref="DirFiles.IncludeMask"/>).
    /// </para>
    /// 	<para>
    /// You can use <see cref="DirFiles.ExcludeMasks"/> if it is required to exclude certain files from
    /// being included into setup.
    /// </para>
    /// <para>
    /// This class is a logical equivalent of <see cref="Files"/> except it analyses files in as single directory.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Note that all files matching wildcard are resolwed into absolute path thus it may not always be suitable 
    /// if the Wix# script is to be compiled into WiX XML source only (Compiler.<see cref="WixSharp.Compiler.BuildWxs(WixSharp.Project)"/>). Though it is not a problem at all if the Wix# script 
    /// is compiled into MSI file (Compiler.<see cref="Compiler.BuildMsi(WixSharp.Project)"/>).
    /// </remarks>
    /// <example>The following is an example of defining installation files with wildcard character pattern.
    /// <code>
    /// new Project("MyProduct",
    ///     new Dir(@"%ProgramFiles%\MyCompany\MyProduct",
    ///         new DirFiles(@"Release\Bin\*.*"),
    ///             new Dir(@"GlobalResources", new DirFiles(@"Release\Bin\GlobalResources\*.*")),
    ///             new Dir(@"Images", new DirFiles(@"Release\Bin\Images\*.*")),
    ///             ...
    /// </code>
    /// </example>
    public partial class DirFiles : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirFiles"/> class.
        /// </summary>
        public DirFiles() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirFiles"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="sourcePath">The relative path to directory source directory. It must include wildcard pattern for files to be included
        /// into MSI (<c>new DirFiles(@"Release\Bin\*.*")</c>).</param>
        public DirFiles(string sourcePath)
        {
            IncludeMask = IO.Path.GetFileName(sourcePath);
            Directory = IO.Path.GetDirectoryName(sourcePath);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DirFiles"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="sourcePath">The relative path to directory source directory. It must include wildcard pattern for files to be included
        /// into MSI (e.g. <c>new DirFiles(@"Release\Bin\*.*")</c>).</param>
        /// <param name="excludeMasks">Wildcard pattern(s) for files to be excluded from MSI 
        /// (e.g. <c>new DirFiles(typical, @"Release\Bin\*.dll", "*.Test.dll", "*.UnitTest.dll")</c>).</param>
        public DirFiles(string sourcePath, params string[] excludeMasks)
        {
            IncludeMask = IO.Path.GetFileName(sourcePath);
            Directory = IO.Path.GetDirectoryName(sourcePath);
            ExcludeMasks = excludeMasks;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DirFiles"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the directory files should be included in.</param>
        /// <param name="sourcePath">The relative path to directory source directory. It must include wildcard pattern for files to be included
        /// into MSI (e.g. <c>new DirFiles(@"Release\Bin\*.*")</c>).</param>
        public DirFiles(Feature feature, string sourcePath)
        {
            IncludeMask = IO.Path.GetFileName(sourcePath);
            Directory = IO.Path.GetDirectoryName(sourcePath);
            Feature = feature;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DirFiles"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the directory files should be included in.</param>
        /// <param name="sourcePath">The relative path to directory source directory. It must include wildcard pattern for files to be included
        /// into MSI (e.g. <c>new DirFiles(@"Release\Bin\*.*")</c>).</param>
        /// <param name="excludeMasks">Wildcard pattern(s) for files to be excluded from MSI 
        /// (e.g. <c>new DirFiles(typical, @"Release\Bin\*.dll", "*.Test.dll", "*.UnitTest.dll")</c>).</param>
        public DirFiles(Feature feature, string sourcePath, params string[] excludeMasks)
        {
            IncludeMask = IO.Path.GetFileName(sourcePath);
            Directory = IO.Path.GetDirectoryName(sourcePath);
            ExcludeMasks = excludeMasks;
            Feature = feature;
        }

        /// <summary>
        /// <see cref="Feature"></see> the directory files are included in.
        /// </summary>
        public Feature Feature;
        /// <summary>
        /// The relative path to directory source directory. To lookup for files matching the <see cref="DirFiles.IncludeMask"/>.
        /// </summary>
        public string Directory = "";
        /// <summary>
        /// Wildcard pattern for files to be included into MSI. 
        /// <para>Default value is <c>*.*</c>.</para>
        /// </summary>
        public string IncludeMask = "*.*";
        /// <summary>
        /// Wildcard patterns for files to be excluded from MSI.
        /// </summary>
        public string[] ExcludeMasks = new string[0];
                
        /// <summary>
        /// Analyses <paramref name="baseDirectory"/> and returns all files matching <see cref="DirFiles.IncludeMask"/>,
        /// which are not matching any <see cref="DirFiles.ExcludeMasks"/>.
        /// </summary>
        /// <param name="baseDirectory">The base directory for file analysis. It is used in conjunction with 
        /// relative <see cref="DirFiles.Directory"/>.</param>
        /// <returns>Array of <see cref="File"/>s</returns>
        public File[] GetFiles(string baseDirectory)
        {
            if (baseDirectory.IsEmpty())
                baseDirectory = System.Environment.CurrentDirectory;

            var files = new List<File>();
            var excludeWildcards = new List<Compiler.Wildcard>();
            foreach (var mask in ExcludeMasks)
                excludeWildcards.Add(new Compiler.Wildcard(mask, RegexOptions.IgnoreCase));

            foreach (string file in IO.Directory.GetFiles(Utils.PathCombine(baseDirectory, Directory), IncludeMask))
            {
                bool ignore = false;
                foreach (Compiler.Wildcard wildcard in excludeWildcards)
                    if ((ignore = wildcard.IsMatch(file)) == true)
                        break;

                if (!ignore)
                {
                    //var filePath = Files.ToRelativePath(IO.Path.GetFullPath(file), baseDirectory);
                    var filePath = IO.Path.GetFullPath(file);
                    Debug.WriteLine(filePath);

                    if (Feature != null)
                        files.Add(new File(Feature, filePath));
                    else
                        files.Add(new File(filePath));
                }
            }
            return files.ToArray();
        }
    }
}
