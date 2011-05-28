package org.embox.robobot.ui;

import org.embox.robobotandroidiface.R;

import android.app.Activity;
import android.os.Bundle;
import android.view.Window;

public class MainSelect extends Activity {
    /** Called when the activity is first created. */

	@Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        setContentView(R.layout.devices);
    }
}