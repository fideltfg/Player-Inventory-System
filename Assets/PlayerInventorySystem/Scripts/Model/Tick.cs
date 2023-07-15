using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Tick : Dictionary<string, float>
{

    public int ID { get { return (int)this["ID"]; } private set { } }
    public float TickRate { get { return this["tickRate"]; } private set { } }
    public float TickScale { get { return this["tickscale"]; } private set { } }
    public float Income { get { return this["income"]; } private set { } }
    public float Expences { get { return this["expences"]; } private set { } }
    public float Growth { get { return this["growth"]; } private set { } }
    public float Ballance { get { return this["ballance"]; } private set { } }
    public float Population { get { return this["population"]; } private set { } }
    public float TaxRate { get { return this["taxrate"]; } private set { } }
    public float FedTaxPaid { get { return this["fedtaxpaid"]; } private set { } }
    public float UpkeepPaid { get { return this["upkeeppaid"]; } private set { } }

    public Tick(int tickID, float tickRate, float tickScale, float population)
    {
        this.Add("ID", tickID);
        this.Add("ballance", 0);
        this.Add("tickrate", tickRate);
        this.Add("tickscale", tickScale);
        this.Add("income", 0);
        this.Add("expences", 0);
        this.Add("growth", 0);
        this.Add("taxrate", 0);
        this.Add("population", population);
        this.Add("fedtaxpaid", 0);
        this.Add("upkeeppaid", 0);
    }


    public enum Value
    {
        GROWTH,
        INCOME,
        EXPENCES,
        BALLANCE,
        TAXRATE,
        POPULATION,
        FEDTAXPAID,
        UPKEEPPAID,
    }

    public float Get(string value)
    {
        return this[value];
    }
    public float Get(Value value)
    {
        switch (value)
        {
            case Value.GROWTH:
                return this["growth"];
            case Value.POPULATION:
                return this["population"];
            case Value.INCOME:
                return this["income"];
            case Value.EXPENCES:
                return this["expences"];
            case Value.BALLANCE:
                return this["ballance"];
            case Value.TAXRATE:
                return this["taxrate"];
            case Value.FEDTAXPAID:
                return this["fedtaxpaid"];
            case Value.UPKEEPPAID:
                return this["upkeeppaid"];
            default: return 0;
        }
    }

    public void Set(Value v, float f)
    {
        switch (v)
        {
            case Value.GROWTH:
                this["growth"] = f;
                break;
            case Value.POPULATION:
                this["growth"] = f;
                break;
            case Value.INCOME:
                this["income"] = f;
                break;
            case Value.EXPENCES:
                this["expences"] = f;
                break;
            case Value.BALLANCE:
                this["ballance"] = f;
                break;
            case Value.TAXRATE:
                this["taxrate"] = f;
                break;
            case Value.FEDTAXPAID:
                this["fedtaxpaid"] = f;
                break;
            case Value.UPKEEPPAID:
                this["upkeeppaid"] = f;
                break;
            default:
                Debug.LogError("oops");
                return;
        }
    }

}