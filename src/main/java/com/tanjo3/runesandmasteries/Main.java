package com.tanjo3.runesandmasteries;

import com.tanjo3.runesandmasteries.view.ChampionFilter;
import com.tanjo3.runesandmasteries.view.GUI;
import com.google.gson.JsonArray;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParser;
import java.awt.Component;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;
import java.io.IOException;
import java.net.URL;
import java.util.Collections;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.Map;
import java.util.Scanner;
import java.util.TreeMap;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.swing.DefaultListModel;
import javax.swing.JCheckBox;
import javax.swing.JLabel;
import javax.swing.JOptionPane;

public class Main {

    private static GUI view;

    private static LinkedList<String> runes;
    private static LinkedList<String> masteries;

    public static void main(String[] args) {
        // set the Nimbus look and feel
        //<editor-fold defaultstate="collapsed" desc=" Look and feel setting code (optional) ">
        /* If Nimbus (introduced in Java SE 6) is not available, stay with the default look and feel.
         * For details see http://download.oracle.com/javase/tutorial/uiswing/lookandfeel/plaf.html 
         */
        try {
            for (javax.swing.UIManager.LookAndFeelInfo info : javax.swing.UIManager.getInstalledLookAndFeels()) {
                if ("Nimbus".equals(info.getName())) {
                    javax.swing.UIManager.setLookAndFeel(info.getClassName());
                    break;
                }
            }
        } catch (ClassNotFoundException | InstantiationException | IllegalAccessException | javax.swing.UnsupportedLookAndFeelException ex) {
            java.util.logging.Logger.getLogger(GUI.class.getName()).log(java.util.logging.Level.SEVERE, null, ex);
        }
        //</editor-fold>

        view = new GUI();
        view.setVisible(true);

        String apiKey = "";

        // ensure API key file is present
        URL url = Main.class.getResource("/api_key");
        if (url == null) {
            Logger.getGlobal().log(Level.SEVERE, "Error Locating API Key");
            JOptionPane.showMessageDialog(null, "Error: API key file not found.", "Error Locating API Key", JOptionPane.ERROR_MESSAGE);
            System.exit(0);
        }

        // read in the API key
        try {
            apiKey = new Scanner(url.openStream()).nextLine();
        } catch (IOException ex) {
            Logger.getGlobal().log(Level.SEVERE, null, ex);
            JOptionPane.showMessageDialog(null, "Error: Unable to read API key from file.", "Error Reading API Key", JOptionPane.ERROR_MESSAGE);
            System.exit(0);
        }

        // generate a list of champion names
        Map<String, String> allChampions = new TreeMap<>();
        try {
            JsonArray allChampData = new JsonParser().parse(API.request("/champion?api_key=" + apiKey)).getAsJsonArray();

            for (JsonElement je : allChampData) {
                allChampions.put(je.getAsJsonObject().getAsJsonPrimitive("name").getAsString(), je.getAsJsonObject().getAsJsonPrimitive("key").getAsString());
            }
        } catch (IOException ex) {
            Logger.getGlobal().log(Level.SEVERE, null, ex);
            JOptionPane.showMessageDialog(null, "Error: Could not connect to the API.", "Error Getting Champions List", JOptionPane.ERROR_MESSAGE);
            System.exit(0);
        }

        // add listeners to the lists
        view.getRunesList().addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent evt) {
                setAsRunes(runes.get(view.getRunesList().locationToIndex(evt.getPoint())));
            }
        });

        view.getMasteriesList().addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent evt) {
                setAsMasteries(masteries.get(view.getMasteriesList().locationToIndex(evt.getPoint())));
            }
        });

        // add listener to "New Query" button
        final String API_KEY = apiKey;
        view.getNewQueryButton().addMouseListener(new MouseAdapter() {
            @Override
            public void mouseClicked(MouseEvent evt) {
                view.resetAll();
                mainRunLoop(API_KEY, allChampions);
            }
        });

        // run the rest of the program
        mainRunLoop(apiKey, allChampions);
    }

    private static void mainRunLoop(final String API_KEY, Map<String, String> championList) {
        // ask user which champions they want to look up
        ChampionFilter champSelect = new ChampionFilter(championList.keySet());
        int result = JOptionPane.showConfirmDialog(null, champSelect, "Champion Filter", JOptionPane.OK_CANCEL_OPTION, JOptionPane.PLAIN_MESSAGE);
        if (result != JOptionPane.OK_OPTION) {
            Logger.getGlobal().log(Level.INFO, "User did not confirm champion selection.");
            System.exit(0);
        }

        // remove undesired champions from map
        Map<String, String> champions = new TreeMap<>(championList);
        for (Component c : champSelect.getChampionsPanel().getComponents()) {
            JCheckBox box = (JCheckBox) c;
            if (!box.isSelected()) {
                champions.remove(box.getText());
            }
        }

        // for each champion the user has selected, get the runes and masteries information from Champion.GG
        Map<String, LinkedList<String>> runesCount = new HashMap<>();
        Map<String, LinkedList<String>> masteryCount = new HashMap<>();

        for (Map.Entry<String, String> champ : champions.entrySet()) {
            try {
                String champKey = champ.getValue();
                String champName = champ.getKey();

                // for each champion, get their data
                JsonArray allData = new JsonParser().parse(API.request("/champion/" + champKey + "?api_key=" + API_KEY)).getAsJsonArray();

                // for each role, get their runes and masteries
                for (JsonElement je : allData) {
                    String role = champName + " " + je.getAsJsonObject().getAsJsonPrimitive("role").getAsString();

                    JsonObject allRunes = je.getAsJsonObject().getAsJsonObject("runes");
                    JsonObject allMasteries = je.getAsJsonObject().getAsJsonObject("masteries");

                    // most frequent runes and masteries
                    if (true) {
                        // get their most frequent runes
                        JsonArray runesMostGames = allRunes.getAsJsonObject("mostGames").getAsJsonArray("runes");
                        if (runesMostGames.size() > 3) {
                            runesCount.computeIfAbsent(runesMostGames.toString(), (k) -> new LinkedList<>()).add(role);
                        } else {
                            Logger.getGlobal().log(Level.WARNING, "Could not get frequent runes info on {0}.", role);
                        }

                        // get their most frequent masteries
                        JsonArray masteryMostGames = allMasteries.getAsJsonObject("mostGames").getAsJsonArray("masteries");
                        if (masteryMostGames.size() == 3) {
                            masteryCount.computeIfAbsent(masteryMostGames.toString(), (k) -> new LinkedList<>()).add(role);
                        } else {
                            Logger.getGlobal().log(Level.WARNING, "Could not get frequent masteries info on {0}.", role);
                        }

                    }

                    // highest winrate runes and masteries (off for now)
                    if (false) {
                        // get their highest winrate runes
                        JsonArray runesHighWins = allRunes.getAsJsonObject("highestWinPercent").getAsJsonArray("runes");
                        if (runesHighWins.size() > 3) {
                            runesCount.computeIfAbsent(runesHighWins.toString(), (k) -> new LinkedList<>()).add(role);
                        } else {
                            Logger.getGlobal().log(Level.WARNING, "Could not get highest winrate runes info on {0}.", role);
                        }

                        // get their highest winrate masteries
                        JsonArray masteryHighWins = allMasteries.getAsJsonObject("highestWinPercent").getAsJsonArray("masteries");
                        if (masteryHighWins.size() == 3) {
                            masteryCount.computeIfAbsent(masteryHighWins.toString(), (k) -> new LinkedList<>()).add(role);
                        } else {
                            Logger.getGlobal().log(Level.WARNING, "Could not get  highest winrate masteries info on {0}.", role);
                        }
                    }
                }
            } catch (IOException ex) {
                Logger.getGlobal().log(Level.SEVERE, null, ex);
                JOptionPane.showMessageDialog(null, "Error: " + ex.getMessage(), "Unable To Get Champion Data", JOptionPane.ERROR_MESSAGE);
            }
        }

        // sort the runes and masteries by their count
        Map<ListCount, String> sortedRunes = new TreeMap<>(Collections.reverseOrder());
        Map<ListCount, String> sortedMastery = new TreeMap<>(Collections.reverseOrder());

        runesCount.entrySet().stream().forEach((entry) -> {
            String runeset = entry.getKey();
            LinkedList<String> champs = entry.getValue();
            sortedRunes.put(new ListCount(champs), runeset);
        });

        masteryCount.entrySet().stream().forEach((entry) -> {
            String masteryset = entry.getKey();
            LinkedList<String> champs = entry.getValue();
            sortedMastery.put(new ListCount(champs), masteryset);
        });

        // add the results to the GUI
        sortedRunes.keySet().stream().forEach((champs) -> {
            ((DefaultListModel) view.getRunesList().getModel()).addElement(champs);
        });

        sortedMastery.keySet().stream().forEach((champs) -> {
            ((DefaultListModel) view.getMasteriesList().getModel()).addElement(champs);
        });

        runes = new LinkedList<>(sortedRunes.values());
        masteries = new LinkedList<>(sortedMastery.values());
    }

    private static void setAsRunes(String runesJSON) {
        StringBuilder runesTxt = new StringBuilder();

        for (JsonElement je : new JsonParser().parse(runesJSON).getAsJsonArray()) {
            JsonObject jo = je.getAsJsonObject();

            StringBuilder sb = new StringBuilder();
            sb.append(jo.getAsJsonPrimitive("number").getAsString());
            sb.append("x ");
            sb.append(jo.getAsJsonPrimitive("name").getAsString());
            sb.append(" (");
            sb.append(jo.getAsJsonPrimitive("description").getAsString());
            sb.append(")\n");

            runesTxt.append(sb.toString());
        }

        view.getRunesOutput().setText(runesTxt.toString());
    }

    private static void setAsMasteries(String masteriesJSON) {
        JsonArray json = new JsonParser().parse(masteriesJSON).getAsJsonArray();
        JsonArray ferocityJSON = json.get(0).getAsJsonObject().getAsJsonArray("data");
        JsonArray cunningJSON = json.get(1).getAsJsonObject().getAsJsonArray("data");
        JsonArray resolveJSON = json.get(2).getAsJsonObject().getAsJsonArray("data");

        view.resetMasteries();

        HashMap<String, JLabel> ferocity = view.getFerocityTree();
        HashMap<String, JLabel> cunning = view.getCunningTree();
        HashMap<String, JLabel> resolve = view.getResolveTree();

        for (JsonElement je : ferocityJSON) {
            JsonObject jo = je.getAsJsonObject();

            JsonElement eleMasteryID = jo.get("mastery");
            JsonElement eleMasteryPts = jo.get("points");

            if (!eleMasteryID.isJsonNull() && !eleMasteryPts.isJsonNull()) {
                String masteryID = eleMasteryID.getAsJsonPrimitive().getAsString();
                String masteryPts = eleMasteryPts.getAsJsonPrimitive().getAsString();
                if (Integer.parseInt(masteryPts) > 0) {
                    ferocity.get(masteryID).setEnabled(true);
                    ferocity.get(masteryID).setText(masteryPts);
                }
            }
        }

        for (JsonElement je : cunningJSON) {
            JsonObject jo = je.getAsJsonObject();

            JsonElement eleMasteryID = jo.get("mastery");
            JsonElement eleMasteryPts = jo.get("points");

            if (!eleMasteryID.isJsonNull() && !eleMasteryPts.isJsonNull()) {
                String masteryID = eleMasteryID.getAsJsonPrimitive().getAsString();
                String masteryPts = eleMasteryPts.getAsJsonPrimitive().getAsString();
                if (Integer.parseInt(masteryPts) > 0) {
                    cunning.get(masteryID).setEnabled(true);
                    cunning.get(masteryID).setText(masteryPts);
                }
            }
        }

        for (JsonElement je : resolveJSON) {
            JsonObject jo = je.getAsJsonObject();

            JsonElement eleMasteryID = jo.get("mastery");
            JsonElement eleMasteryPts = jo.get("points");

            if (!eleMasteryID.isJsonNull() && !eleMasteryPts.isJsonNull()) {
                String masteryID = eleMasteryID.getAsJsonPrimitive().getAsString();
                String masteryPts = eleMasteryPts.getAsJsonPrimitive().getAsString();
                if (Integer.parseInt(masteryPts) > 0) {
                    resolve.get(masteryID).setEnabled(true);
                    resolve.get(masteryID).setText(masteryPts);
                }
            }
        }
    }
}
