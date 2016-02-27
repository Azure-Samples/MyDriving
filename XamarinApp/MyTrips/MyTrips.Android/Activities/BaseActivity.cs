using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using MyTrips.Droid.Helpers;
using Plugin.Permissions;
using Android.Content.PM;

namespace MyTrips.Droid
{
    public abstract class BaseActivity : AppCompatActivity, IAccelerometerListener
    {
        public Toolbar Toolbar { get; set; }
        protected abstract int LayoutResource { get; }

        protected int ActionBarIcon
        {
            set { Toolbar.SetNavigationIcon(value); }
        }

        AccelerometerManager accelerometerManager;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            InitActivityTransitions();
            SetContentView(LayoutResource);
            Toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            if (Toolbar != null)
            {
                SetSupportActionBar(Toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);

            }

            accelerometerManager = new AccelerometerManager(this, this);
        }



        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnAccelerationChanged(float x, float y, float z)
        {

        }
        bool canShowFeedback;
        public void OnShake(float force)
        {
            if (!canShowFeedback)
                return;
            canShowFeedback = false;
            HockeyApp.FeedbackManager.ShowFeedbackActivity(this);
        }


        protected override void OnResume()
        {
            base.OnResume();
            canShowFeedback = true;
            if (accelerometerManager.IsSupported)
                accelerometerManager.StartListening();
            
        }

        void InitActivityTransitions() 
        {
            if ((int)Build.VERSION.SdkInt >= 21) 
            {
                var transition = new Android.Transitions.Slide();
                transition.ExcludeTarget(Android.Resource.Id.StatusBarBackground, true);
                Window.EnterTransition = transition;
                Window.ReturnTransition = transition;
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            if (accelerometerManager.IsListening)
                accelerometerManager.StopListening();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (accelerometerManager.IsListening)
                accelerometerManager.StopListening();
        }

       
    }
}