
Original Author, Source and License
-----------------------------------

Author: Bill Menees
Source: http://www.menees.com/

Diff.Net
--------

Diff.Net is a differencing utility I wrote in C#.
It provides side-by-side differencing for files and directories.
For files it also provides an overview diff and a line-to-line diff.
And it can do a visual difference of binary files.

This software is CharityWare.
If you use it, I ask that you make a $5 US donation to the charity of your choice.
Its binaries and source code are licensed using the Apache License, Version 2.0.

http://www.apache.org/licenses/LICENSE-2.0.

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS"
BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the License for the specific language
governing permissions and limitations under the License.

License CharityWare
-------------------

All of the software directly downloadable from this site is CharityWare.

It is not freeware, shareware, or commercial software.
CharityWare means that if you use the software, you agree to make at least a $5 US donation to some worthwhile charity
of your choice.

I’ll never know if you don’t follow this licensing policy, but the negative karma from illegally using this software will
be far worse than giving $5 to help someone else out. And if you do follow this policy, the good karma you accumulate will
be much better than anything else you could get for $5.

By "worthwhile charity" I mean a charity that helps other people, animals, or life in general. This can be your religious group,
your local pet shelter, a save-the-planet foundation, etc. Anything that applies compassion and loving-kindness with wisdom
toward the benefit of other beings and life will do fine. There are opportunities to do this all around you. In addition
to the big-name charities and organizations (e.g. United Way, Salvation Army, Greenpeace, etc.), at almost every convenience store,
fast food restaurant, etc. there are donation baskets for some good cause.

I wrote this software to help everyone out, and all I ask is that you return the favor by helping someone else out. Thanks!

Documentation
-------------

- BurnsBinaryDiff_Paper.pdf
- BurnsBinaryDiff_Thesis.pdf
- MyersDiff.pdf

Menees.Diffs - Non-visual, GUI-agnostic classes to find differences between text sequences,
binary sequences, and directories. Text differencing is performed using the algorithm from An O(ND)
Difference Algorithm and Its Variations by Eugene W. Myers. Binary differencing is performed using
the algorithm from A Linear Time, Constant Space Differencing Algorithm by:

Randal C. Burns and Darrell D. E. Long as well as from
Differential Compression: A Generalized Solution For Binary Files by Randal C. Burns.

The output of a binary diff can also be written as a GDIFF stream.