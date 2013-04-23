package org.embox.robobot.ui;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import org.embox.robobot.R;

/**
 * Created with IntelliJ IDEA.
 * User: vloginova
 * Date: 23.04.13
 * Time: 14:09
 * To change this template use File | Settings | File Templates.
 */
public class MainMenuActivity extends Activity {
    private static final String TAG = MainMenuActivity.class.getSimpleName();

    private Button internetButton;
    private Button bluetoothButton;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.d(TAG, "onCreate");
        setContentView(R.layout.main_menu);
        internetButton = (Button) findViewById(R.id.internet);
        bluetoothButton = (Button) findViewById(R.id.bluetooth);

        internetButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View arg0) {
                Intent intent = new Intent(MainMenuActivity.this, AddressChooserActivity.class);
                startActivity(intent);
            }
        });

        bluetoothButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View arg0) {
                Intent intent = new Intent(MainMenuActivity.this, SelectActivity.class);
                startActivity(intent);
            }
        });
    }
}
