//
// Filename:  TermCap.cs
// 
// Module: Unix.Console.TermCap.cs
//
// Decription:  Provide access to terminal capabilities database
//
// Author:
//	Jonathan Stowe <jns@gellyfish.com>
//
// Copyright (c) 2004 Jonathan Stowe <jns@gellyfish.com>
//

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
// $Log: TermCap.cs,v $
// Revision 1.2  2004/09/07 20:47:15  jonathan
// Added the NoCapabilityException
//
// Revision 1.1.1.1  2004/09/07 20:14:12  jonathan
// Added To CVS
//
// 

using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace Unix.Console
{
   using System;
	public class TermCap
	{
	
	   Hashtable caps;
	
	   private string capfile = "/etc/termcap";
	
	   public string Entry;
	   public string[] Items;
	
	   public TermCap()
	   {
         caps = new Hashtable();
	      this.Entry = this.GetEntry();
	      this.Items = this.Entry.Split(new char[] {':'});
	      foreach (string it in this.Items)
	      {
            if (it.Length > 0 )
            {
               Capability cap = new Capability(it);
               caps[cap.Name] = cap;
            }
	      }
	   }
	
      public string Home
      {
         get
         {
            return this.Put("ho");
         }
      }

      public string Clear
      {
         get
         {
            return this.Put("cl");
         }
      }

      public string StartStandout
      {
         get
         {
            return this.Put("so");
         }
      }

      public string EndStandout
      {
         get
         {
            return this.Put("se");
         }
      }

      public string Put(string cap)
      {
         if (caps.Contains(cap))
         {
            Capability c = (Capability)caps[cap];
            return c.Value;
         }
         else
         {
            throw new NoCapabilityException(cap);
         }
      }

	   private  string GetEntry()
	   {
	      StreamReader s = new StreamReader(capfile);
	
	      bool first;
	      bool more = true;
	      string term = Environment.GetEnvironmentVariable("TERM");
	      string entry = "";
	      while (more)
	      {
	         string l;
	         s.BaseStream.Seek(0,SeekOrigin.Begin);
	         term = Regex.Escape(term);
	         Regex tt = new Regex("(^|\\|)" +term + "[:|]");
	         Regex stripfirst = new Regex("^[^:]*:");
	         Regex nextline = new Regex("\\\\$");
	         Regex nextTermCap = new Regex(":tc=(?<tc>[^:]+)");
	         while((l = s.ReadLine()) != null )
	         {
	            first = false;
	            if (l.StartsWith("#"))
	            {
	               continue;
	            }
	            if (tt.IsMatch(l))
	            {
	               if ( first )
	               {
	                  l = stripfirst.Replace(l,"");
	                  first = true;
	               }
	
	               string rr = "";
	               while ( (l = nextline.Replace(l,"")) != rr  )
	               {
	                  string x;
	                  if ((x = s.ReadLine()) == null )
	                  {
	                     break;
	                  }
	                  l += x;
	                  rr = l;
	
	               }
	
	               l = Regex.Replace(l,":\\s+","");
	               if (nextTermCap.IsMatch(l))
	               {
	                  term = nextTermCap.Match(l).Groups["tc"].Value;
	                  l = nextTermCap.Replace(l,"");
	                  more = true;
	               }
	               else
	               {
	                  more = false;
	               }
	
	               break; 
	            }
	         }
	         l = stripfirst.Replace(l,"");
	         entry += l;
	      }
	      return entry;
	   }
	}
	
	public class Capability
	{
	
	   public string Name;
	   public string Value;
	   public CapabilityType Type;
	
	   public Capability(string cap)
	   {
	      string[] capsplit = cap.Split(new char[] {'='});
	      this.Name = capsplit[0];
	      if ( capsplit.Length > 1 )
	      {
	         this.Type = CapabilityType.String;
	         this.Value = this.ProcessValue(capsplit[1]);
	      }
	      else
	      {
	         if ( Regex.IsMatch("#",cap))
	         {
	            capsplit = cap.Split(new char[] {'#'});
	            this.Name = capsplit[0];
	            this.Value = capsplit[1];
	            this.Type = CapabilityType.Numeric;
	         }
	         else
	         {
	            this.Type = CapabilityType.Boolean;
	         }
	      }
	   }

      private string ProcessValue(string val)
      {
         val = Regex.Replace(val,@"\E","");
         val = Regex.Replace(val,@"\n","\n");
         val = Regex.Replace(val,@"\t","\t");
         val = Regex.Replace(val,@"\r","\r");
         val = Regex.Replace(val,@"\b","\b");
         val = Regex.Replace(val,@"\f","\f");

         return val;

      }
	}
	
	public enum  CapabilityType :int
	{
	   String   =  0,
	   Numeric  =  1,
	   Boolean  =  2
	}

   public class NoCapabilityException :Exception
   {
      public NoCapabilityException(string cap) : base( "Capability " + cap + "not present for this terminal")
      {
      }

    
   }

}
