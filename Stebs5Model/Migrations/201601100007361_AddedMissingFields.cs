namespace Stebs5Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMissingFields : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Files", newName: "FileSystemNodes");
            AddColumn("dbo.FileSystemNodes", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.FileSystemNodes", "Folder_Id", c => c.Long());
            AddColumn("dbo.FileSystemNodes", "FileSystem_Id", c => c.Long());
            AddColumn("dbo.FileSystems", "Root_Id", c => c.Long());
            CreateIndex("dbo.FileSystemNodes", "Folder_Id");
            CreateIndex("dbo.FileSystemNodes", "FileSystem_Id");
            CreateIndex("dbo.FileSystems", "Root_Id");
            AddForeignKey("dbo.FileSystemNodes", "Folder_Id", "dbo.FileSystemNodes", "Id");
            AddForeignKey("dbo.FileSystemNodes", "FileSystem_Id", "dbo.FileSystems", "Id");
            AddForeignKey("dbo.FileSystems", "Root_Id", "dbo.FileSystemNodes", "Id");
            DropTable("dbo.Folders");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Folders",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.FileSystems", "Root_Id", "dbo.FileSystemNodes");
            DropForeignKey("dbo.FileSystemNodes", "FileSystem_Id", "dbo.FileSystems");
            DropForeignKey("dbo.FileSystemNodes", "Folder_Id", "dbo.FileSystemNodes");
            DropIndex("dbo.FileSystems", new[] { "Root_Id" });
            DropIndex("dbo.FileSystemNodes", new[] { "FileSystem_Id" });
            DropIndex("dbo.FileSystemNodes", new[] { "Folder_Id" });
            DropColumn("dbo.FileSystems", "Root_Id");
            DropColumn("dbo.FileSystemNodes", "FileSystem_Id");
            DropColumn("dbo.FileSystemNodes", "Folder_Id");
            DropColumn("dbo.FileSystemNodes", "Discriminator");
            RenameTable(name: "dbo.FileSystemNodes", newName: "Files");
        }
    }
}
