namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

public class VolunteerImplementation : IVolunteer
{
    private readonly string _filePath = "volunteers.xml";

    public void Create(Volunteer item)
    {
        XElement volunteers = XElement.Load(_filePath);
        XElement newVolunteer = new XElement("Volunteer",
            new XElement("Id", item.Id),
            new XElement("FullName", item.FullName),
            new XElement("PhoneNumber", item.PhoneNumber),
            new XElement("Email", item.Email),
            new XElement("Password", item.Password),
            new XElement("CurrentAddress", item.CurrentAddress),
            new XElement("Latitude", item.Latitude),
            new XElement("Longitude", item.Longitude),
            new XElement("MaxDistance", item.MaxDistance),
            new XElement("VolunteerRole", item.VolunteerRole),
            new XElement("IsActive", item.IsActive),
            new XElement("DistanceType", item.DistanceType)
        );
        volunteers.Add(newVolunteer);
        volunteers.Save(_filePath);
    }

    public void Delete(int id)
    {
        XElement volunteers = XElement.Load(_filePath);
        XElement? volunteer = volunteers.Elements("Volunteer")
            .FirstOrDefault(v => (int?)v.Element("Id") == id);
        if (volunteer != null)
        {
            volunteer.Remove();
            volunteers.Save(_filePath);
        }
    }

    public void DeleteAll()
    {
        XElement volunteers = new XElement("Volunteers");
        volunteers.Save(_filePath);
    }

    public Volunteer? Read(int id)
    {
        XElement volunteers = XElement.Load(_filePath);
        XElement? volunteer = volunteers.Elements("Volunteer")
            .FirstOrDefault(v => (int?)v.Element("Id") == id);
        return volunteer != null ? ParseVolunteer(volunteer) : null;
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        XElement volunteers = XElement.Load(_filePath);
        return volunteers.Elements("Volunteer")
            .Select(v => ParseVolunteer(v))
            .FirstOrDefault(filter);
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        XElement volunteers = XElement.Load(_filePath);
        IEnumerable<Volunteer> volunteerList = volunteers.Elements("Volunteer")
            .Select(v => ParseVolunteer(v));
        return filter != null ? volunteerList.Where(filter) : volunteerList;
    }

    public void Update(Volunteer item)
    {
        XElement volunteers = XElement.Load(_filePath);
        XElement? volunteer = volunteers.Elements("Volunteer")
            .FirstOrDefault(v => (int?)v.Element("Id") == item.Id);
        if (volunteer != null)
        {
            volunteer.SetElementValue("FullName", item.FullName);
            volunteer.SetElementValue("PhoneNumber", item.PhoneNumber);
            volunteer.SetElementValue("Email", item.Email);
            volunteer.SetElementValue("Password", item.Password);
            volunteer.SetElementValue("CurrentAddress", item.CurrentAddress);
            volunteer.SetElementValue("Latitude", item.Latitude);
            volunteer.SetElementValue("Longitude", item.Longitude);
            volunteer.SetElementValue("MaxDistance", item.MaxDistance);
            volunteer.SetElementValue("VolunteerRole", item.VolunteerRole);
            volunteer.SetElementValue("IsActive", item.IsActive);
            volunteer.SetElementValue("DistanceType", item.DistanceType);
            volunteers.Save(_filePath);
        }
    }

    private Volunteer ParseVolunteer(XElement element)
    {
        return new Volunteer(
            (int?)element.Element("Id") ?? 0,
            (string?)element.Element("FullName") ?? string.Empty,
            (string?)element.Element("PhoneNumber") ?? "0000000000",
            (string?)element.Element("Email") ?? string.Empty,
            (string?)element.Element("Password"),
            (string?)element.Element("CurrentAddress"),
            (double?)element.Element("Latitude"),
            (double?)element.Element("Longitude"),
            (double?)element.Element("MaxDistance"),
            (Role)Enum.Parse(typeof(Role), (string?)element.Element("VolunteerRole") ?? Role.Volunteer.ToString()),
            (bool?)element.Element("IsActive") ?? true,
            (DistanceType)Enum.Parse(typeof(DistanceType), (string?)element.Element("DistanceType") ?? DistanceType.AirDistance.ToString())
        );
    }
}
