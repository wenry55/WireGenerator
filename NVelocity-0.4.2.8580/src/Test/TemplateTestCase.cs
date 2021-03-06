using System;
using System.IO;

using TestProvider = NVelocity.Test.Provider.TestProvider;
using BoolObj = NVelocity.Test.Provider.BoolObj;
using System.Collections;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;

using NUnit.Framework;

using Commons.Collections;

namespace NVelocity.Test {

    /// <summary>
    /// Easily add test cases which evaluate templates and check their output.
    ///
    /// NOTE:
    /// This class DOES NOT extend RuntimeTestCase because the TemplateTestSuite
    /// already initializes the Velocity runtime and adds the template
    /// test cases. Having this class extend RuntimeTestCase causes the
    /// Runtime to be initialized twice which is not good. I only discovered
    /// this after a couple hours of wondering why all the properties
    /// being setup were ending up as Vectors. At first I thought it
    /// was a problem with the Configuration class, but the Runtime
    /// was being initialized twice: so the first time the property
    /// is seen it's stored as a String, the second time it's seen
    /// the Configuration class makes a Vector with both Strings.
    /// As a result all the getBoolean(property) calls were failing because
    /// the Configurations class was trying to create a Boolean from
    /// a Vector which doesn't really work that well. I have learned
    /// my lesson and now have to add some code to make sure the
    /// Runtime isn't initialized more then once :-)
    /// </summary>
    /// <author> <a href="mailto:dlr@finemaltcoding.com">Daniel Rall</a>
    /// </author>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author> <a href="mailto:jon@latchkey.com">Jon S. Stevens</a>
    /// </author>
    [TestFixture]
    public class TemplateTestCase : BaseTestCase {

	private ExtendedProperties testProperties;

	private TestProvider provider;
	private ArrayList al;
	private System.Collections.Hashtable h;
	private VelocityContext context;
	private VelocityContext context1;
	private VelocityContext context2;
	private System.Collections.ArrayList vec;

	/// <summary>
	/// Creates a new instance.
	/// </summary>
	public TemplateTestCase() {
	    try {
		Velocity.SetProperty(RuntimeConstants_Fields.FILE_RESOURCE_LOADER_PATH, NVelocity.Test.TemplateTestBase_Fields.FILE_RESOURCE_LOADER_PATH);

		Velocity.SetProperty(RuntimeConstants_Fields.RUNTIME_LOG_ERROR_STACKTRACE, "true");
		Velocity.SetProperty(RuntimeConstants_Fields.RUNTIME_LOG_WARN_STACKTRACE, "true");
		Velocity.SetProperty(RuntimeConstants_Fields.RUNTIME_LOG_INFO_STACKTRACE, "true");

		Velocity.Init();

		testProperties = new ExtendedProperties();
		testProperties.Load(new FileStream(TemplateTestBase_Fields.TEST_CASE_PROPERTIES, FileMode.Open, FileAccess.Read));
	    } catch (System.Exception e) {
		throw new System.Exception("Cannot setup TemplateTestSuite!");
	    }
	}

	/// <summary>
	/// Sets up the test.
	/// </summary>
	[SetUp]
	protected void SetUp() {
	    provider = new TestProvider();
	    al = provider.Customers;
	    h = new System.Collections.Hashtable();

	    SupportClass.PutElement(h, "Bar", "this is from a hashtable!");
	    SupportClass.PutElement(h, "Foo", "this is from a hashtable too!");

	    /*
	    *  lets set up a vector of objects to test late introspection. See ASTMethod.java
	    */

	    vec = new System.Collections.ArrayList();

	    vec.Add(new System.String("string1".ToCharArray()));
	    vec.Add(new System.String("string2".ToCharArray()));

	    /*
	    *  set up 3 chained contexts, and add our data 
	    *  throught the 3 of them.
	    */

	    context2 = new VelocityContext();
	    context1 = new VelocityContext(context2);
	    context = new VelocityContext(context1);

	    context.Put("provider", provider);
	    context1.Put("name", "jason");
	    context2.Put("providers", provider.Customers2);
	    context.Put("list", al);
	    context1.Put("hashtable", h);
	    context2.Put("hashmap", new Hashtable());
	    context2.Put("search", provider.Search);
	    context.Put("relatedSearches", provider.RelSearches);
	    context1.Put("searchResults", provider.RelSearches);
	    context2.Put("stringarray", provider.Array);
	    context.Put("vector", vec);
	    context.Put("mystring", new System.String("".ToCharArray()));
	    context.Put("runtime", new FieldMethodizer("NVelocity.Runtime.RuntimeSingleton"));
	    context.Put("fmprov", new FieldMethodizer(provider));
	    context.Put("Floog", "floogie woogie");
	    context.Put("boolobj", new BoolObj());

	    /*
	    *  we want to make sure we test all types of iterative objects
	    *  in #foreach()
	    */

	    System.Object[] oarr = new System.Object[]{"a", "b", "c", "d"};
	    int[] intarr = new int[]{10, 20, 30, 40, 50};

	    context.Put("collection", vec);
	    context2.Put("iterator", vec.GetEnumerator());
	    context1.Put("map", h);
	    context.Put("obarr", oarr);
	    context.Put("enumerator", vec.GetEnumerator());
	    context.Put("intarr", intarr);
	}

	/// <summary>
	/// Adds the template test cases to run to this test suite.  Template test
	/// cases are listed in the <code>TEST_CASE_PROPERTIES</code> file.
	/// </summary>
	[Test]
	public void Test_Run() {
	    System.String template;
	    Boolean allpass = true;
	    Int32 failures = 0;
	    for (int i = 1; ; i++) {
		template = (System.String)
		testProperties.GetString(getTemplateTestKey(i));

		if (template != null) {
		    Boolean pass = RunTest(template)
		    ;
		    if (!pass) {
			System.Console.Out.Write("Adding TemplateTestCase : " + template + "...")
			;
			Console.Out.WriteLine("FAIL!");
			allpass = false;
			failures++;
			if (failures>100) {
			    Assertion.Fail();
			}
		    }
		}
		else {
		    // Assume we're done adding template test cases.
		    break;
		}
	    }
	    if (!allpass) {
		Assertion.Fail(failures.ToString() + " templates failed");
	    }
	}

	/// <summary>
	/// Macro which returns the properties file key for the specified template
	/// test number.
	/// </summary>
	/// <param name="nbr">The template test number to return a property key for.</param>
	/// <returns>The property key.</returns>
	private static System.String getTemplateTestKey(int nbr) {
	    return ("test.template." + nbr);
	}

	/// <summary>
	/// Runs the test.
	/// </summary>
	private Boolean RunTest(String baseFileName) {
	    // run setup before each test so that the context is clean
	    SetUp();

	    try {
		Template template = RuntimeSingleton.getTemplate(getFileName(null, baseFileName, NVelocity.Test.TemplateTestBase_Fields.TMPL_FILE_EXT))
		;

		assureResultsDirectoryExists(NVelocity.Test.TemplateTestBase_Fields.RESULT_DIR);

		/* get the file to write to */
		FileStream fos = new FileStream(getFileName(NVelocity.Test.TemplateTestBase_Fields.RESULT_DIR, baseFileName, NVelocity.Test.TemplateTestBase_Fields.RESULT_FILE_EXT), FileMode.Create);

		//UPGRADE_ISSUE: Constructor 'java.io.BufferedWriter.BufferedWriter' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javaioBufferedWriterBufferedWriter_javaioWriter"'
		StreamWriter writer = new StreamWriter(fos);

		/* process the template */
		template.Merge(context, writer);

		/* close the file */
		writer.Flush();
		writer.Close();

		if (!isMatch(TemplateTestBase_Fields.RESULT_DIR, TemplateTestBase_Fields.COMPARE_DIR, baseFileName,
			     TemplateTestBase_Fields.RESULT_FILE_EXT, TemplateTestBase_Fields.CMP_FILE_EXT)) {
		    //Fail("Processed template did not match expected output");
		    return false;
		}
	    } catch (System.Exception e) {
		System.Console.Out.WriteLine("EXCEPTION : " + e);

		Assertion.Fail(e.Message);
	    }
	    return true;
	}


    }
}
