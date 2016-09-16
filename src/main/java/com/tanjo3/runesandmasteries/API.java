package com.tanjo3.runesandmasteries;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;

public class API {

    /**
     * The API to which we are sending requests.
     */
    private static final String API = "http://api.champion.gg";

    /**
     * You can not create an object of this class.
     */
    private API() {
    }

    /**
     * Sends a request to the API.
     *
     * @param request the formatted request to send (minus API address)
     * @return a string representing the reply from the API
     * @throws IOException if an I/O error occurs
     */
    public static String request(String request) throws IOException {
        // connect to the API
        URL url = new URL(API + request);
        HttpURLConnection httpcon = (HttpURLConnection) url.openConnection();
        httpcon.addRequestProperty("User-Agent", "Mozilla");

        // read the response into a string
        StringBuilder result = new StringBuilder();
        try (BufferedReader rd = new BufferedReader(new InputStreamReader(httpcon.getInputStream()))) {
            String line;
            while ((line = rd.readLine()) != null) {
                result.append(line);
            }
        }

        // disconnect from the API
        httpcon.disconnect();

        return result.toString();
    }
}
