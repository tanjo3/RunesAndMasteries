package com.tanjo3.runesandmasteries.view;

import java.awt.Component;
import java.awt.Font;
import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.Set;
import javax.swing.BorderFactory;
import javax.swing.ButtonGroup;
import javax.swing.GroupLayout;
import javax.swing.JCheckBox;
import javax.swing.JPanel;
import javax.swing.JRadioButton;
import javax.swing.LayoutStyle;
import javax.swing.border.TitledBorder;

public class ChampionFilter extends JPanel {

    /**
     * Creates new panel for selecting champions.
     *
     * @param champions a list of champion names
     */
    public ChampionFilter(Set<String> champions) {
        initComponents();

        populateWithChampions(champions);
    }

    /**
     * This method is called from within the constructor to initialize the form.
     * WARNING: Do NOT modify this code. The content of this method is always
     * regenerated by the Form Editor.
     */
    @SuppressWarnings("unchecked")
    // <editor-fold defaultstate="collapsed" desc="Generated Code">//GEN-BEGIN:initComponents
    private void initComponents() {

        grpSelection = new ButtonGroup();
        btnSpecific = new JRadioButton();
        btnAll = new JRadioButton();
        pnlChampions = new JPanel();

        grpSelection.add(btnSpecific);
        btnSpecific.setFont(new Font("Felix Titling", 0, 11)); // NOI18N
        btnSpecific.setSelected(true);
        btnSpecific.setText("Query Specific Champions");

        grpSelection.add(btnAll);
        btnAll.setFont(new Font("Felix Titling", 0, 11)); // NOI18N
        btnAll.setText("Select All Champions");
        btnAll.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent evt) {
                btnAllActionPerformed(evt);
            }
        });

        pnlChampions.setBorder(BorderFactory.createTitledBorder(null, "Champions", TitledBorder.DEFAULT_JUSTIFICATION, TitledBorder.DEFAULT_POSITION, new Font("Felix Titling", 0, 14))); // NOI18N
        pnlChampions.setLayout(new GridLayout(0, 10));

        GroupLayout layout = new GroupLayout(this);
        this.setLayout(layout);
        layout.setHorizontalGroup(layout.createParallelGroup(GroupLayout.Alignment.LEADING)
            .addGroup(layout.createSequentialGroup()
                .addContainerGap()
                .addGroup(layout.createParallelGroup(GroupLayout.Alignment.LEADING)
                    .addComponent(pnlChampions, GroupLayout.DEFAULT_SIZE, GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE)
                    .addGroup(layout.createSequentialGroup()
                        .addComponent(btnSpecific)
                        .addGap(18, 18, 18)
                        .addComponent(btnAll)
                        .addGap(0, 266, Short.MAX_VALUE)))
                .addContainerGap())
        );
        layout.setVerticalGroup(layout.createParallelGroup(GroupLayout.Alignment.LEADING)
            .addGroup(layout.createSequentialGroup()
                .addContainerGap()
                .addGroup(layout.createParallelGroup(GroupLayout.Alignment.BASELINE)
                    .addComponent(btnSpecific)
                    .addComponent(btnAll))
                .addPreferredGap(LayoutStyle.ComponentPlacement.RELATED)
                .addComponent(pnlChampions, GroupLayout.DEFAULT_SIZE, 437, Short.MAX_VALUE)
                .addContainerGap())
        );
    }// </editor-fold>//GEN-END:initComponents

    private void populateWithChampions(Set<String> champions) {
        champions.stream().forEach((c) -> {
            JCheckBox box = new JCheckBox(c);
            box.setFont(new Font("Felix Titling", 0, 11));
            box.addActionListener((evt) -> {
                btnSpecific.setSelected(true);
            });
            pnlChampions.add(box);
        });
    }

    private void btnAllActionPerformed(ActionEvent evt) {//GEN-FIRST:event_btnAllActionPerformed
        for (Component c : pnlChampions.getComponents()) {
            ((JCheckBox) c).setSelected(true);
        }
    }//GEN-LAST:event_btnAllActionPerformed

    public JPanel getChampionsPanel() {
        return pnlChampions;
    }

    // Variables declaration - do not modify//GEN-BEGIN:variables
    private JRadioButton btnAll;
    private JRadioButton btnSpecific;
    private ButtonGroup grpSelection;
    private JPanel pnlChampions;
    // End of variables declaration//GEN-END:variables
}
